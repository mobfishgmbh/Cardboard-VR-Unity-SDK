#if (UNITY_IOS || UNITY_ANDROID) && !UNITY_EDITOR
#define NATIVE_PLUGIN_EXIST
#endif

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

namespace MobfishCardboard
{
    public static class CardboardDistortionRenderer
    {
        private static IntPtr _cardboardDistortionRenderer;

        #if NATIVE_PLUGIN_EXIST
        [DllImport(CardboardUtility.DLLName)]
        private static extern IntPtr CardboardDistortionRenderer_create();

        [DllImport(CardboardUtility.DLLName)]
        private static extern void CardboardDistortionRenderer_destroy(IntPtr renderer);

        [DllImport(CardboardUtility.DLLName)]
        private static extern void CardboardDistortionRenderer_setMesh(
            IntPtr renderer, ref CardboardMesh mesh, CardboardEye eye);

        [DllImport(CardboardUtility.DLLName)]
        private static extern void CardboardDistortionRenderer_renderEyeToDisplay(IntPtr renderer,
            int target_display, int rectX, int rectY, int display_width, int display_height,
            CardboardEyeTextureDescription left_eye, CardboardEyeTextureDescription right_eye);

        #else

        private static IntPtr CardboardDistortionRenderer_create()
        {
            return IntPtr.Zero;
        }

        private static void CardboardDistortionRenderer_destroy(IntPtr renderer)
        {
        }

        private static void CardboardDistortionRenderer_setMesh(
            IntPtr renderer, ref CardboardMesh mesh, CardboardEye eye)
        {
        }

        private static void CardboardDistortionRenderer_renderEyeToDisplay(IntPtr renderer,
            int target_display, int rectX, int rectY, int display_width, int display_height,
            CardboardEyeTextureDescription left_eye, CardboardEyeTextureDescription right_eye)
        {
        }

        #endif

        public static void InitDestortionRenderer()
        {
            _cardboardDistortionRenderer = CardboardDistortionRenderer_create();
        }

        public static void SetEyeMeshes(CardboardMesh leftMeshPtr, CardboardMesh rightMeshPtr)
        {
            CardboardDistortionRenderer_setMesh(_cardboardDistortionRenderer, ref leftMeshPtr, CardboardEye.kLeft);
            CardboardDistortionRenderer_setMesh(_cardboardDistortionRenderer, ref rightMeshPtr, CardboardEye.kRight);
        }

        public static void RenderEyeToDisplay(
            CardboardEyeTextureDescription left_eye, CardboardEyeTextureDescription right_eye)
        {
            CardboardDistortionRenderer_renderEyeToDisplay(_cardboardDistortionRenderer, 0, 0, 0,
                Screen.width, Screen.height, left_eye, right_eye);
        }
    }
}