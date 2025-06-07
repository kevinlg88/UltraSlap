using UnityEngine;
using Rewired;
using System.Collections.Generic;

public class RewiredJoinManager : MonoBehaviour
{
    private void Update()
    {
        if (!ReInput.isReady) return;

        if (ReInput.controllers.Keyboard.GetAnyButtonDown())
        {
            foreach (Player player in ReInput.players.Players)
            {
                if (!player.controllers.hasKeyboard &&
                    player.controllers.joystickCount == 0)
                {
                    player.controllers.AddController(ReInput.controllers.Keyboard, false);
                    Debug.Log($"Teclado atribuído ao Player {player.id}");
                    break;
                }
            }
        }

        foreach (Joystick joy in ReInput.controllers.Joysticks)
        {
            if (joy.GetAnyButtonDown())
            {
                Player player = FindAvailablePlayer();
                if (player != null && !player.controllers.ContainsController(joy))
                {
                    player.controllers.AddController(joy, false);
                    Debug.Log($"Joystick {joy.name} atribuído ao Player {player.id}");
                }
            }
        }
    }
    private Player FindAvailablePlayer()
    {
        foreach (Player player in ReInput.players.AllPlayers)
        {
            if (player.controllers.joystickCount > 0 ||
                player.controllers.hasKeyboard)
            {
                continue;
            }
            return player;
        }
        return null;
    }

}

