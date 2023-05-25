using System;
using Agora.Rtc;
using Agora.Util;
using UnityEngine;

public class VoiceChatController : MonoBehaviour
{
    public const string APP_ID = "aaa4dba26f6f4befb235ccb1aec6a3a4";
    public static VoiceChatController Instance { get; private set; }

    private IRtcEngine _engine;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        RtcEngineContext context = new RtcEngineContext(APP_ID, 0,
                                    CHANNEL_PROFILE_TYPE.CHANNEL_PROFILE_LIVE_BROADCASTING,
                                    AUDIO_SCENARIO_TYPE.AUDIO_SCENARIO_DEFAULT);

        _engine = RtcEngine.CreateAgoraRtcEngine();
        _engine.Initialize(context);
        
        SetMute(false);
    }

    private void Start()
    {
        _engine.SetChannelProfile(CHANNEL_PROFILE_TYPE.CHANNEL_PROFILE_COMMUNICATION);
        _engine.SetClientRole(CLIENT_ROLE_TYPE.CLIENT_ROLE_BROADCASTER);

        LobbyManager.Instance.OnJoinedLobby += OnJoinedLobby;
        LobbyManager.Instance.OnLeftLobby += OnLeftLobby;
    }

    private void Update()
    {
        PermissionHelper.RequestMicrophontPermission();
    }

    public void SetMute(bool mute)
    {
        _engine.AdjustRecordingSignalVolume(mute ? 0 : 120);
    }

    private void OnJoinedLobby(object sender, LobbyEventArgs args)
    {
        JoinChannel(args.lobby.Name);
    }

    private void OnLeftLobby(object sender, EventArgs e)
    {
        LeaveChannel();
    }

    private void JoinChannel(string channelName)
    {
        _engine.EnableAudio();
        _engine.AdjustRecordingSignalVolume(120);
        _engine.JoinChannel("", channelName);
    }

    private void LeaveChannel()
    {
        _engine.DisableAudio();
        _engine.LeaveChannel();
    }

    private void OnApplicationQuit()
    {
        if (_engine != null)
        {
            LeaveChannel();

            _engine.Dispose();
            _engine = null;
        }
    }
}
