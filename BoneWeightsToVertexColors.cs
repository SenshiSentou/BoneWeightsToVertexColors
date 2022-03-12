using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.U2D;
using Unity.Collections;

namespace Ares.Examples {
    public class BoneWeightsToVertexColors : MonoBehaviour {
        enum AutoRunMoment { Never, Awake, Start}

        [SerializeField] AutoRunMoment autoRun;
        [SerializeField] string rBoneName;
        [SerializeField] string gBoneName;
        [SerializeField] string bBoneName;
        [SerializeField] string aBoneName;

        void Awake() {
            if(autoRun == AutoRunMoment.Awake) {
                Generate();
            }
        }

        void Start() {
            if(autoRun == AutoRunMoment.Start) {
                Generate();
            }
        }

        [ContextMenu("Generate Vertex Color Data")]
        public void Generate() {
            Sprite sprite = GetComponent<SpriteRenderer>().sprite;
            SpriteBone[] bones = sprite.GetBones();
            NativeSlice<BoneWeight> weights = sprite.GetVertexAttribute<BoneWeight>(VertexAttribute.BlendWeight);
            Color32[] colors = Enumerable.Repeat(new Color32(0, 0, 0, 255), sprite.GetVertexCount()).ToArray();

            void GenerateChannel(int channel, string boneName) {
                if(string.IsNullOrEmpty(boneName)) {
                    return;
                }

                SpriteBone bone = bones.FirstOrDefault(b => b.name.ToLower() == boneName.ToLower());
                int boneIndex = System.Array.IndexOf(bones, bone);

                if(boneIndex < 0) {
                    Debug.LogWarning($"Generating vertex colors for {boneName} failed; no bone with this name was found.");
                }

                for(int i = 0; i < weights.Length; i++) {
                    Color color = colors[i];
                    BoneWeight weight = weights[i];

                    if(weight.boneIndex0 == boneIndex)      color[channel] = weight.weight0 * 255;
                    else if(weight.boneIndex1 == boneIndex) color[channel] = weight.weight1 * 255;
                    else if(weight.boneIndex2 == boneIndex) color[channel] = weight.weight2 * 255;
                    else if(weight.boneIndex3 == boneIndex) color[channel] = weight.weight3 * 255;

                    colors[i] = color;
                }
            }

            GenerateChannel(0, rBoneName);
            GenerateChannel(1, gBoneName);
            GenerateChannel(2, bBoneName);
            GenerateChannel(3, aBoneName);

            sprite.SetVertexAttribute(VertexAttribute.Color, new NativeArray<Color32>(colors, Allocator.Temp));
        }
    }
}