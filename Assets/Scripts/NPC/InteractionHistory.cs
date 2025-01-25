using Mono.Collections.Generic;
using System.Collections.Generic;
using UnityEngine;

public class InteractionHistory : MonoBehaviour
{
    private List<Interaction> receivedInteractions = new List<Interaction>();

    public bool HasInteractedWith(Interaction interaction)
    {
        return receivedInteractions.Contains(interaction);
    }

    protected void AddHasBeenInteractedWith(Interaction interaction)
    {
        if (!receivedInteractions.Contains(interaction))
        {
            receivedInteractions.Add(interaction);
        }
    }
}
