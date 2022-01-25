using System;
using UnityEngine;

public class PhantomGraphicController : MonoBehaviour
{
    [Serializable]
    public class ModeMaterialsDictionary : UnitySerializedDictionary<MovementMode, Material> { }

    public MeshRenderer body;
    public MeshRenderer eyes;
    public ModeMaterialsDictionary modeMaterials = new ModeMaterialsDictionary();

    public void SetMode(MovementMode mode)
    {
        if (modeMaterials.TryGetValue(mode, out var material))
        {
            body.material = material;
        }
    }
}