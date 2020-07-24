using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using EPOOutline;
using System;

#if SRP_OUTLINE
#if UNITY_2019_3_OR_NEWER
using UnityEngine.Rendering.Universal;
#else
using UnityEngine.Rendering.LWRP;
#endif

#if UNITY_2019_2_OR_NEWER
namespace SRPOutline
{
#endif
    public class SRPOutlineFeature : ScriptableRendererFeature
    {
        private class SRPOutline : ScriptableRenderPass, IGraphicsControlProvider
        {
            private List<Outlinable> outlinables = new List<Outlinable>();

            public OutlineParameters Parameters = new OutlineParameters();

            public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
            {
                Parameters.ControlProvider = this;
                Parameters.Width = cameraTextureDescriptor.width;
                Parameters.Height = cameraTextureDescriptor.height;
                Parameters.Antialiasing = cameraTextureDescriptor.msaaSamples;
            }

            public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
            {
                Predicate<Outlinable> editorPredicate = obj =>
                    {
#if UNITY_EDITOR
                        var stage = UnityEditor.Experimental.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage();

                        return stage == null || stage.IsPartOfPrefabContents(obj.gameObject);
#else
                        return true;
#endif
                    };

                Predicate<Outlinable> gamePredicate = obj =>                   
                    {
#if UNITY_EDITOR
                        var stage = UnityEditor.Experimental.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage();

                        return stage == null || !stage.IsPartOfPrefabContents(obj.gameObject);
#else
                        return true;
#endif
                    };

                Parameters.DrawPredicate = renderingData.cameraData.isSceneViewCamera ? editorPredicate : gamePredicate;
                var buffer = CommandBufferPool.Get("Outline");

                outlinables.Clear();
                Parameters.Layers.Clear();
                Outlinable.GetAllActiveOutlinables(outlinables);
                foreach (var outlinable in outlinables)
                    Parameters.Layers.Add(outlinable.Layer);

                Parameters.Buffer = buffer;
                OutlineEffect.SetupBuffer(Parameters);

                context.ExecuteCommandBuffer(buffer);

                CommandBufferPool.Release(buffer);
            }

            public override void FrameCleanup(CommandBuffer cmd)
            {
                cmd.Clear();
            }

            public void ClearTarget(bool clearDepth, bool clearColor, Color color)
            {
                var flags = (clearDepth ? ClearFlag.Depth : ClearFlag.None) | (clearColor ? ClearFlag.Color : ClearFlag.None);

                ConfigureClear(flags, color);
            }

            public void SetTarget(RenderTargetIdentifier target)
            {
                Parameters.Buffer.SetRenderTarget(target);
            }

            public void DrawRenderer(Renderer renderer, Material material, int submesh, bool useCutoutVersion)
            {
                Parameters.Buffer.DrawRenderer(renderer, material, submesh, useCutoutVersion ? 1 : 0);
            }

            public void Blit(RenderTargetIdentifier first, RenderTargetIdentifier second, Material material, int pass = -1)
            {
                Parameters.Buffer.Blit(first, second, material, pass);
            }
        }

        [System.Serializable]
        public class Settings
        {
            [Range(0.05f, 1.0f)] public float PrimaryRendererScale = 1.0f;
            [Range(0.05f, 1.0f)] public float InfoBufferScale = 1.0f;
            [SerializeField, Range(0, 10)] public int BlurIterations = 1;
            [SerializeField, Range(0, 10)] public int DilateIterations = 1;

            [SerializeField, Range(0.0f, 3.0f)] public float BlurShift = 1.0f;
            [SerializeField, Range(0.0f, 3.0f)] public float DilateShift = 1.0f;

            public bool UsingInfoBuffer = false;

            public bool SustainedPerformanceMode = true;
        }

        private SRPOutline pass;

        [SerializeField] private Settings settings = new Settings();

        public override void Create()
        {
            pass = new SRPOutline();
            pass.renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            pass.Parameters.UseHDR = renderingData.cameraData.isHdrEnabled;

            pass.Parameters.Target = renderer.cameraColorTarget;
            pass.Parameters.PrimaryRendererScale = settings.PrimaryRendererScale;
            pass.Parameters.InfoBufferScale = settings.InfoBufferScale;
            pass.Parameters.UsingInfoBuffer = settings.UsingInfoBuffer;
            pass.Parameters.BlurIterations = settings.BlurIterations;
            pass.Parameters.DilateIterations = settings.DilateIterations;
            pass.Parameters.BlurShift = settings.BlurShift;
            pass.Parameters.DilateShift = settings.DilateShift;
            pass.Parameters.Depth = renderer.cameraDepth;
            pass.Parameters.SustainedPerformanceMode = settings.SustainedPerformanceMode;

            renderer.EnqueuePass(pass);
        }
}
#if  UNITY_2019_2_OR_NEWER
}
#endif
#endif