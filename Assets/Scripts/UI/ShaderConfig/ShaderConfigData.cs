
public static class ShaderConfig
{
    public static float TextureInfluence = 0f;
    public static float BrightnessToAlpha = 1f;
    public static float ScanLineCount = 128f;
    public static float FlickerIntensity = 0.25f;

    public static void ResetToDefaults()
    {
        TextureInfluence = 0f;
        BrightnessToAlpha = 1f;
        ScanLineCount = 128f;
        FlickerIntensity = 0.25f;
    }
}
