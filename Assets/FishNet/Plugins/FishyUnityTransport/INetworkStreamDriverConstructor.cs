// // Decompiled with JetBrains decompiler
// // Type: Unity.Netcode.Transports.UTP.INetworkStreamDriverConstructor
// // Assembly: Unity.Netcode.Runtime, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// // MVID: BF55477A-83FB-4B7C-B9CA-15CED647E8C8
// // Assembly location: C:\Users\Alven\Documents\Projects\com.unity.multiplayer.samples.coop\Library\ScriptAssemblies\Unity.Netcode.Runtime.dll
//
// using Unity.Networking.Transport;
//
// namespace FishNet.Transporting.FishyUnityTransport
// {
//     public interface INetworkStreamDriverConstructor
//     {
//         void CreateDriver(
//             FishyUnityTransport transport,
//             out NetworkDriver driver,
//             out NetworkPipeline unreliableFragmentedPipeline,
//             out NetworkPipeline unreliableSequencedFragmentedPipeline,
//             out NetworkPipeline reliableSequencedPipeline, bool asServer);
//     }
// }