using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

namespace N.Package.Interactives.Components
{
    public class NControlPointer : MonoBehaviour
    {
        InputAction _cachedAxis;

        InputAction _cachedTrigger;

        /// <summary>
        /// Leave this at -1 for single-player games.
        /// For multi-player games, set this to be the player index, and the actions will
        /// be read from that player's controls
        /// </summary>
        [Tooltip("Leave this at -1 for single-player games. For multiplayer set it to the player id")]
        public int playerIndex = -1;

        [Tooltip("Vector2 action for XY position; note use position not delta for this input!")]
        public InputActionReference axis;

        [Tooltip("Button action to trigger this action as on or off")]
        public InputActionReference trigger;

        public void Awake()
        {
            GetAxisValue(); // Enable axis if it is not enabled yet
        }

        public void OnTrigger(Action<InputAction.CallbackContext> onStarted, Action<InputAction.CallbackContext> onCancelled)
        {
            var triggerAction = ResolveAction(trigger, ActionId.Trigger);
            if (triggerAction == null) return;
            triggerAction.started += onStarted;
            triggerAction.canceled += onCancelled;
        }

        public void RemoveEventHandler(Action<InputAction.CallbackContext> onStarted, Action<InputAction.CallbackContext> onCancelled)
        {
            var triggerAction = ResolveAction(trigger, ActionId.Trigger);
            if (triggerAction == null) return;

            if (onStarted != null)
            {
                triggerAction.started -= onStarted;
            }

            if (onCancelled != null)
            {
                triggerAction.canceled -= onCancelled;
            }
        }

        public Vector2 GetAxisValue()
        {
            var action = ResolveAction(axis, ActionId.Axis);
            if (action != null)
            {
                return action.ReadValue<Vector2>();
            }

            return default;
        }

        private InputAction ResolveAction(InputActionReference actionRef, ActionId id)
        {
            if (actionRef == null || actionRef.action == null)
                return null;

            if (Get(id) != null && actionRef.action.id != Get(id).id)
                Set(id, null);

            if (Get(id) == null)
            {
                Set(id, actionRef.action);
                if (playerIndex != -1)
                {
                    var user = InputUser.all[playerIndex];
                    Set(id, user.actions.First(x => x.id == actionRef.action.id));
                }
            }

            // Auto-enable it if disabled
            if (Get(id) != null && !Get(id).enabled)
                Get(id).Enable();

            return Get(id);
        }

        protected virtual void OnDisable()
        {
            _cachedAxis = null;
        }

        private InputAction Get(ActionId id)
        {
            switch (id)
            {
                case ActionId.Axis:
                    return _cachedAxis;
                case ActionId.Trigger:
                    return _cachedTrigger;
                default:
                    throw new ArgumentOutOfRangeException(nameof(id), id, null);
            }
        }

        private void Set(ActionId id, InputAction action)
        {
            switch (id)
            {
                case ActionId.Axis:
                    _cachedAxis = action;
                    break;
                case ActionId.Trigger:
                    _cachedTrigger = action;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(id), id, null);
            }
        }

        private enum ActionId
        {
            Axis = 0,
            Trigger = 1
        }
    }
}