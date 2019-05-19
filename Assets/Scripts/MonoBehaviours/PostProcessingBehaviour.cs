using UnityEngine;

namespace PostProcessing
{
    [RequireComponent(typeof(Camera))]
    [ImageEffectAllowedInSceneView]
    [ExecuteInEditMode]
    public class PostProcessingBehaviour : MonoBehaviour
    {
        public PostProcessingCollection collection;

        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            if (collection && collection.postProcessingMaterials.Count > 0)
            {
                for (int i = 0; i < collection.postProcessingMaterials.Count; i++)
                {
                    if (collection.postProcessingMaterials[i]) Graphics.Blit(source, destination, collection.postProcessingMaterials[i]);
                }
            }
            else
            {
                Graphics.Blit(source, dest: null);
            }
        }
    }
}
