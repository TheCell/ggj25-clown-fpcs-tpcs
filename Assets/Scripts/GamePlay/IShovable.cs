using System.Collections;
using UnityEngine;

namespace GamePlay
{
    public interface IShovable
    {
        IEnumerator GetShoved(Vector3 shoveDirection);
    }
}
