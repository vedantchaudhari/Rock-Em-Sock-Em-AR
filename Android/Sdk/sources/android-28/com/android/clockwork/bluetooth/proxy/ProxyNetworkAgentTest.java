package com.android.clockwork.bluetooth.proxy;

import static org.junit.Assert.assertEquals;
import static org.junit.Assert.assertNotEquals;
import static org.junit.Assert.assertNotNull;
import static org.junit.Assert.assertNull;
import static org.junit.Assert.assertTrue;
import static org.junit.Assert.fail;
import static org.mockito.Matchers.any;
import static org.mockito.Matchers.anyInt;
import static org.mockito.Matchers.anyString;
import static org.mockito.Mockito.atLeast;
import static org.mockito.Mockito.never;
import static org.mockito.Mockito.verify;
import static org.mockito.Mockito.when;

import android.content.Context;
import android.net.LinkProperties;
import android.net.NetworkAgent;
import android.net.NetworkAgentHelper; // Testable helper
import android.net.NetworkCapabilities;
import android.net.NetworkInfo;
import android.os.RemoteException;
import com.android.clockwork.WearRobolectricTestRunner;
import com.android.internal.util.IndentingPrintWriter;
import java.lang.reflect.Field;
import java.util.Hashtable;
import org.junit.Before;
import org.junit.Test;
import org.junit.runner.RunWith;
import org.mockito.Mock;
import org.mockito.MockitoAnnotations;
import org.robolectric.annotation.Config;
import org.robolectric.shadows.ShadowApplication;

/** Test for {@link ProxyNetworkAgent} */
@RunWith(WearRobolectricTestRunner.class)
@Config(manifest = Config.NONE,
        shadows = {ShadowNetworkInfo.class, ShadowConnectivityManager.class },
        sdk = 26)
public class ProxyNetworkAgentTest {
    final ShadowApplication shadowApplication = ShadowApplication.getInstance();

    private static final int NETWORK_SCORE = 123;
    private static final String COMPANION_NAME = "Companion Name";
    private static final String REASON = "Reason";

    @Mock IndentingPrintWriter mockIndentingPrintWriter;
    @Mock LinkProperties mockLinkProperties;
    @Mock NetworkAgent mockNetworkAgent;
    @Mock NetworkCapabilities mockCapabilities;
    @Mock NetworkInfo mockNetworkInfo;
    @Mock ProxyLinkProperties mockProxyLinkProperties;

    private ProxyNetworkAgent mProxyNetworkAgent;

    @Before
    public void setUp() throws RemoteException {
        MockitoAnnotations.initMocks(this);

        when(mockProxyLinkProperties.getLinkProperties()).thenReturn(mockLinkProperties);
        Hashtable<String, LinkProperties> stackedLinks = new Hashtable<String, LinkProperties>();
        try {
            Field field = LinkProperties.class.getDeclaredField("mStackedLinks");
            field.setAccessible(true);
            field.set(mockLinkProperties, stackedLinks);

        } catch (NoSuchFieldException | IllegalAccessException e) {
            fail();
        }

        final Context context = ShadowApplication.getInstance().getApplicationContext();
        mProxyNetworkAgent = new ProxyNetworkAgent(
                context,
                mockCapabilities,
                mockProxyLinkProperties);
    }

    @Test
    public void testSetUpNetworkAgent_NoAgent() {
        mProxyNetworkAgent.mCurrentNetworkAgent = null;

        mProxyNetworkAgent.setUpNetworkAgent(REASON, COMPANION_NAME, null);
        assertEquals(1, mProxyNetworkAgent.mNetworkAgents.size());
    }

    @Test
    public void testSetUpNetworkAgent_ExistingAgentReUse() {
        setupNetworkAgent();

        mProxyNetworkAgent.setUpNetworkAgent(REASON, COMPANION_NAME, null);
        assertEquals(2, mProxyNetworkAgent.mNetworkAgents.size());
        assertNotEquals(mockNetworkAgent, mProxyNetworkAgent.mCurrentNetworkAgent);
    }

    @Test
    public void testSetUpNetworkAgent_ExistingAgentForceNew() {
        setupNetworkAgent();

        mProxyNetworkAgent.setUpNetworkAgent(REASON, COMPANION_NAME, null);
        assertEquals(2, mProxyNetworkAgent.mNetworkAgents.size());
        assertNotEquals(mockNetworkAgent, mProxyNetworkAgent.mCurrentNetworkAgent);
    }

    @Test
    public void testMaybeSetUpNetworkAgent_NoAgent() {
        mProxyNetworkAgent.mCurrentNetworkAgent = null;

        mProxyNetworkAgent.maybeSetUpNetworkAgent(REASON, COMPANION_NAME, null);
        assertEquals(1, mProxyNetworkAgent.mNetworkAgents.size());
    }

    @Test
    public void testMaybeSetUpNetworkAgent_ExistingAgentReUse() {
        setupNetworkAgent();

        mProxyNetworkAgent.maybeSetUpNetworkAgent(REASON, COMPANION_NAME, null);
        assertEquals(1, mProxyNetworkAgent.mNetworkAgents.size());
        assertEquals(mockNetworkAgent, mProxyNetworkAgent.mCurrentNetworkAgent);
    }

    @Test
    public void testMaybeSetUpNetworkAgent_ExistingAgentForceNew() {
        setupNetworkAgent();

        mProxyNetworkAgent.maybeSetUpNetworkAgent(REASON, COMPANION_NAME, null);
        assertEquals(1, mProxyNetworkAgent.mNetworkAgents.size());
        assertEquals(mockNetworkAgent, mProxyNetworkAgent.mCurrentNetworkAgent);
    }

    @Test
    public void testTearDownNetworkAgent_NoAgentForceNew() {
        mProxyNetworkAgent.mCurrentNetworkAgent = null;

        mProxyNetworkAgent.setUpNetworkAgent(REASON, COMPANION_NAME, null);
        assertEquals(1, mProxyNetworkAgent.mNetworkAgents.size());
        assertNotNull(mProxyNetworkAgent.mCurrentNetworkAgent);

        NetworkAgentHelper.callUnwanted(mProxyNetworkAgent.mCurrentNetworkAgent);

        assertTrue(mProxyNetworkAgent.mNetworkAgents.isEmpty());
        assertNull(mProxyNetworkAgent.mCurrentNetworkAgent);
    }

    @Test
    public void testTearDownNetworkAgent_ExistingAgentForceNew() {
        mProxyNetworkAgent.setUpNetworkAgent(REASON, COMPANION_NAME, null);
        assertEquals(1, mProxyNetworkAgent.mNetworkAgents.size());
        assertNotNull(mProxyNetworkAgent.mCurrentNetworkAgent);

        NetworkAgent unwantedAgent = mProxyNetworkAgent.mCurrentNetworkAgent;

        mProxyNetworkAgent.setUpNetworkAgent(REASON, COMPANION_NAME, null);
        assertEquals(2, mProxyNetworkAgent.mNetworkAgents.size());

        NetworkAgentHelper.callUnwanted(unwantedAgent);

        assertEquals(1, mProxyNetworkAgent.mNetworkAgents.size());
    }

    @Test
    public void testTearDownNetworkAgent_ExistingAgentForceNewButMissingFromHash() {
        mProxyNetworkAgent.setUpNetworkAgent(REASON, COMPANION_NAME, null);
        assertEquals(1, mProxyNetworkAgent.mNetworkAgents.size());
        assertNotNull(mProxyNetworkAgent.mCurrentNetworkAgent);

        NetworkAgent unwantedAgent = mProxyNetworkAgent.mCurrentNetworkAgent;

        mProxyNetworkAgent.setUpNetworkAgent(REASON, COMPANION_NAME, null);
        assertEquals(2, mProxyNetworkAgent.mNetworkAgents.size());

        // Secretly poison the hash here
        mProxyNetworkAgent.mNetworkAgents.remove(unwantedAgent);

        NetworkAgentHelper.callUnwanted(unwantedAgent);

        assertEquals(1, mProxyNetworkAgent.mNetworkAgents.size());
    }

    @Test
    public void testSetConnected_NoAgent() {
        mProxyNetworkAgent.mCurrentNetworkAgent = null;
        mProxyNetworkAgent.setConnected(REASON, COMPANION_NAME);

        verify(mockNetworkInfo, never()).setDetailedState(any(), anyString(), anyString());
        assertTrue(mProxyNetworkAgent.mNetworkAgents.isEmpty());
        verify(mockNetworkAgent, never()).sendNetworkInfo(mockNetworkInfo);
    }

    @Test
    public void testSetConnected_ExistingAgent() {
        setupNetworkAgent();

        mProxyNetworkAgent.setConnected(REASON, COMPANION_NAME);

        verify(mockNetworkAgent).sendNetworkInfo(mockNetworkInfo);
    }

    @Test
    public void testSendCapabilities_NoAgent() {
        mProxyNetworkAgent.mCurrentNetworkAgent = null;
        mProxyNetworkAgent.sendCapabilities(mockCapabilities);
        verify(mockNetworkAgent, never()).sendNetworkCapabilities(mockCapabilities);
    }

    @Test
    public void testSendCapabilities_ExistingAgent() {
        mProxyNetworkAgent.mCurrentNetworkAgent = mockNetworkAgent;
        mProxyNetworkAgent.sendCapabilities(mockCapabilities);
        verify(mockNetworkAgent).sendNetworkCapabilities(mockCapabilities);
    }

    @Test
    public void testSendNetworkScore_NoAgent() {
        mProxyNetworkAgent.mCurrentNetworkAgent = null;
        mProxyNetworkAgent.setNetworkScore(NETWORK_SCORE);

        verify(mockNetworkAgent, never()).sendNetworkScore(NETWORK_SCORE);
    }

    @Test
    public void testSendNetworkScore_ExistingAgent() {
        mProxyNetworkAgent.mCurrentNetworkAgent = mockNetworkAgent;
        mProxyNetworkAgent.setNetworkScore(NETWORK_SCORE);

        verify(mockNetworkAgent).sendNetworkScore(NETWORK_SCORE);
    }


    @Test
    public void testDump_NoAgent() {
        mProxyNetworkAgent.mCurrentNetworkAgent = null;
        mProxyNetworkAgent.dump(mockIndentingPrintWriter);
        verify(mockIndentingPrintWriter).printPair(anyString(), anyString());
    }

    @Test
    public void testDump_ExistingAgent() {
        mProxyNetworkAgent.mCurrentNetworkAgent = mockNetworkAgent;
        mProxyNetworkAgent.dump(mockIndentingPrintWriter);
        verify(mockIndentingPrintWriter, atLeast(1)).printPair(anyString(), anyInt());
    }

    private void setupNetworkAgent() {
        mProxyNetworkAgent.mCurrentNetworkAgent = mockNetworkAgent;
        mProxyNetworkAgent.mNetworkAgents.put(mockNetworkAgent, mockNetworkInfo);
    }

}
