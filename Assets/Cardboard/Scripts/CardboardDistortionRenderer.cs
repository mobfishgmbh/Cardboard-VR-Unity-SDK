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

        [DllImport(CardboardUtility.DLLName)]
        private static extern IntPtr CardboardDistortionRenderer_create();

        [DllImport(CardboardUtility.DLLName)]
        private static extern IntPtr CardboardDistortionRenderer_destroy(IntPtr renderer);

        [DllImport(CardboardUtility.DLLName)]
        private static extern void CardboardDistortionRenderer_setMesh(
            IntPtr renderer, IntPtr mesh, CardboardEye eye);

        [DllImport(CardboardUtility.DLLName)]
        private static extern void CardboardDestortionRenderer_renderEyeToDisplay(IntPtr renderer,
            int target_display, int display_width, int display_height,
            CardboardEyeTextureDescription left_eye, CardboardEyeTextureDescription right_eye);

        public static void InitDestortionRenderer()
        {
            _cardboardDistortionRenderer = CardboardDistortionRenderer_create();
        }

        public static void SetEyeMeshes(IntPtr leftMeshPtr, IntPtr rightMeshPtr)
        {
            CardboardDistortionRenderer_setMesh(_cardboardDistortionRenderer, leftMeshPtr, CardboardEye.kLeft);
            CardboardDistortionRenderer_setMesh(_cardboardDistortionRenderer, rightMeshPtr, CardboardEye.kRight);
        }

        public static void RenderEyeToDisplay(
            CardboardEyeTextureDescription left_eye, CardboardEyeTextureDescription right_eye)
        {
            CardboardDestortionRenderer_renderEyeToDisplay(_cardboardDistortionRenderer, 0, Screen.width, Screen.height,
                left_eye, right_eye);
        }
    }
}