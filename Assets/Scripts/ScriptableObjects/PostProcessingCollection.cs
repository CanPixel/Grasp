using UnityEngine;
using System.Collections.Generic;

namespace PostProcessing
{
    [CreateAssetMenu(menuName = "Post Processing Collection")]
    public class PostProcessingCollection : ScriptableObject
    {
        public List<Material> postProcessingMaterials;
    }
}