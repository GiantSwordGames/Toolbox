// using System;
// using NaughtyAttributes;
// using UnityEngine;
//
// namespace GiantSword
// {
//     public class ScopedStateBase
//     {
//         
//     }
//     public class ScopedState<T> :ScopedStateBase
//     {
//         private Action _initialValue;
//
//         [ShowNativeProperty]  public Action value
//         {
//             get
//             {
//                 if (Application.isPlaying)
//                 {
//                     ScopedEventManager.GetState(this, out ScopedEventManager.State state);
//                     return state.value;
//                 }
//                 else
//                 {
//                     return default;
//                 }
//             }
//             set
//             {
//                 ScopedEventManager.GetState(this, out ScopedEventManager.State<T> state);
//                 if (state == null)
//                 {
//                 }
//                 
//                 if (value == null)
//                 {
//                     Debug.Log("value is null");
//                     return;
//                 }
//                 
//                 if (value.Equals( state.value) == false)
//                 {
//                     state.value = value;
//                 }
//             }
//         }
//         
//         // implicit operator
//         public static implicit operator T(ScopedState<T> scopedState)
//         {
//             return scopedState.value;
//         }
//     }
// }