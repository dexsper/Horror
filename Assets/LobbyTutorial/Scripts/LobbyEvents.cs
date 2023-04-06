using System;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;

public class LobbyEventArgs : EventArgs
{
    public Lobby lobby;
}

public class OnLobbyListChangedEventArgs : EventArgs
{
    public List<Lobby> lobbyList;
}