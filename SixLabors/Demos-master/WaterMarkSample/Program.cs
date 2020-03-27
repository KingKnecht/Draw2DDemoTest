using System;
using System.Numerics;
using ImageSharp;
using SixLabors.Fonts;

namespace WaterMarkSample
{
    class Program
    {
        static void Main(string[] args)
        {
            FontFamily family = FontCollection.SystemFonts.Find("Arial");

            Font font = new Font(family, 100f, FontStyle.Bold);

            ApplyWaterMarkSimple(font, "Copyright Person Name", "small.jpg", "small_simple.jpg");
            ApplyWaterMarkSimple(font, "Copyright Person Name", "large.jpg", "large_simple.jpg");

            ApplyWaterMarkScaled(font, "Copyright Person Name", "small.jpg", "small_scaled.jpg");
            ApplyWaterMarkScaled(font, "Copyright Person Name", "large.jpg", "large_scaled.jpg");


            ApplyWaterMarkScaledWordWrap(font, @"Lorem ipsum dolor sit amet, consectetur adipiscing elit. Donec aliquet lorem at magna mollis, non semper erat aliquet. In leo tellus, sollicitudin non eleifend et, luctus vel magna. Proin at lacinia tortor, malesuada molestie nisl. Quisque mattis dui quis eros ultricies, quis faucibus turpis dapibus. Donec urna ipsum, dignissim eget condimentum at, condimentum non magna. Donec non urna sit amet lectus tincidunt interdum vitae vitae leo. Aliquam in nisl accumsan, feugiat ipsum condimentum, scelerisque diam. Vivamus quam diam, rhoncus ut semper eget, gravida in metus.

Nullam quis malesuada metus. In hac habitasse platea dictumst. Aliquam faucibus eget eros nec vulputate. Quisque sed dolor lacus. Proin non dolor vitae massa rhoncus vestibulum non a arcu. Morbi mollis, arcu id pretium dictum, augue dui cursus eros, eu pharetra arcu ante non lectus. Integer quis tellus ipsum. Integer feugiat augue id tempus rutrum. Ut eget interdum leo, id fermentum lacus. Morbi euismod, mi at tempus finibus, ante risus ornare eros, eu ultrices ipsum dolor vitae risus. Mauris molestie pretium massa vitae maximus. Fusce ut egestas ex, vitae semper nulla. Proin pretium elit libero, et interdum enim molestie ac.

Pellentesque fermentum vitae lacus non aliquet. Sed nulla ipsum, hendrerit sit amet vulputate varius, volutpat eget est. Pellentesque eget ante erat. Vestibulum venenatis ex quis pretium sagittis. Etiam vel nibh sit amet leo gravida efficitur. In hac habitasse platea dictumst. Nullam lobortis euismod sem dapibus aliquam. Proin accumsan velit a magna gravida condimentum. Nam non massa ac nibh viverra rutrum. Phasellus elit tortor, malesuada et purus nec, placerat mattis neque. Proin auctor risus vel libero ultrices, id fringilla erat facilisis. Donec rutrum, enim sit amet faucibus viverra, velit tellus aliquam tellus, et tempus tellus diam sed dui. Integer fringilla convallis nisl venenatis elementum. Sed volutpat massa ut mauris accumsan, mollis finibus tortor pretium.", "large.jpg", "large_wrapped.jpg");

            ApplyWaterMarkScaledWordWrap(font, @"Lorem ipsum dolor sit amet, consectetur adipiscing elit. Donec aliquet lorem at magna mollis, non semper erat aliquet. In leo tellus, sollicitudin non eleifend et, luctus vel magna. Proin at lacinia tortor, malesuada molestie nisl. Quisque mattis dui quis eros ultricies, quis faucibus turpis dapibus. Donec urna ipsum, dignissim eget condimentum at, condimentum non magna. Donec non urna sit amet lectus tincidunt interdum vitae vitae leo. Aliquam in nisl accumsan, feugiat ipsum condimentum, scelerisque diam. Vivamus quam diam, rhoncus ut semper eget, gravida in metus.

Nullam quis malesuada metus. In hac habitasse platea dictumst. Aliquam faucibus eget eros nec vulputate. Quisque sed dolor lacus. Proin non dolor vitae massa rhoncus vestibulum non a arcu. Morbi mollis, arcu id pretium dictum, augue dui cursus eros, eu pharetra arcu ante non lectus. Integer quis tellus ipsum. Integer feugiat augue id tempus rutrum. Ut eget interdum leo, id fermentum lacus. Morbi euismod, mi at tempus finibus, ante risus ornare eros, eu ultrices ipsum dolor vitae risus. Mauris molestie pretium massa vitae maximus. Fusce ut egestas ex, vitae semper nulla. Proin pretium elit libero, et interdum enim molestie ac.

Pellentesque fermentum vitae lacus non aliquet. Sed nulla ipsum, hendrerit sit amet vulputate varius, volutpat eget est. Pellentesque eget ante erat. Vestibulum venenatis ex quis pretium sagittis. Etiam vel nibh sit amet leo gravida efficitur. In hac habitasse platea dictumst. Nullam lobortis euismod sem dapibus aliquam. Proin accumsan velit a magna gravida condimentum. Nam non massa ac nibh viverra rutrum. Phasellus elit tortor, malesuada et purus nec, placerat mattis neque. Proin auctor risus vel libero ultrices, id fringilla erat facilisis. Donec rutrum, enim sit amet faucibus viverra, velit tellus aliquam tellus, et tempus tellus diam sed dui. Integer fringilla convallis nisl venenatis elementum. Sed volutpat massa ut mauris accumsan, mollis finibus tortor pretium.", "small.jpg", "small_wrapped.jpg");
        }

        static void ApplyWaterMarkSimple(Font font, string text, string inputPath, string outputPath)
        {
            using (Image img = Image.Load(inputPath))
            {
                Color fill = new Color(128, 128, 128, 200);

                img.DrawText(text, font, fill, new Vector2(0, 0));

                img.Save(outputPath);
                
            }
        }

        static void ApplyWaterMarkScaled(Font font, string text, string inputPath, string outputPath)
        {
            using (Image img = Image.Load(inputPath))
            {
                TextMeasurer measurer = new TextMeasurer();

                // we can now get the dimensions of the bounding box of a piece of text
                SixLabors.Fonts.Size size = measurer.MeasureText(text, font, 72);

                //  calculate the scaling factor we need to change the fontsize by to fit the image
                float scalingFactor = Math.Min(img.Width / size.Width, img.Height / size.Height);

                Font scaledFont = new Font(font, scalingFactor * font.Size);
                Color fill = new Color(128, 128, 128, 200);

                img.DrawText(text, scaledFont
                    , fill, new Vector2(0, 0));

                img.Save(outputPath);
            }
        }

        static void ApplyWaterMarkScaledWordWrap(Font font, string text, string inputPath, string outputPath)
        {
            using (Image img = Image.Load(inputPath))
            {
                float padding = 20; // margin in 20px

                float targetWidth = img.Width - (padding * 2);
                float targetHeight = img.Height - (padding * 2);

                float targetMinHeight = img.Height - (padding * 3); // must be with in a margin width of the target height

                TextMeasurer measurer = new TextMeasurer();
                
                // now we are working i 2 dimensions at once and cant just scale because it will cause the text to 
                // reflow we need to just try multiple times

                var f = font;
                SixLabors.Fonts.Size s = new SixLabors.Fonts.Size(float.MaxValue, float.MaxValue);

                float scaleFactor = (f.Size / 2);// everytime we change direction we half this size
                int trapCount = (int)f.Size * 2;
                if (trapCount < 10)
                {
                    trapCount = 10;
                }

                bool isTooSmall = false;

                while ((s.Height > targetHeight || s.Height < targetMinHeight) && trapCount > 0)
                {
                    if (s.Height > targetHeight)
                    {
                        if (isTooSmall)
                        {
                            scaleFactor = scaleFactor / 2;
                        }

                        f = new Font(f, f.Size - scaleFactor);
                        isTooSmall = false;
                    }

                    if (s.Height < targetMinHeight)
                    {
                        if (!isTooSmall)
                        {
                            scaleFactor = scaleFactor / 2;
                        }
                        f = new Font(f, f.Size + scaleFactor);
                        isTooSmall = true;
                    }
                    trapCount--;

                    var style = new FontSpan(f, 72)
                    {
                        WrappingWidth = targetWidth
                    };

                    s = measurer.MeasureText(text, style);
                }

                Color fill = new Color(128, 128, 128, 200);

                img.DrawText(text, f
                    , fill, new Vector2(padding), new ImageSharp.Drawing.TextGraphicsOptions {

                        WrapTextWidth = targetWidth
                    });

                img.Save(outputPath);
            }
        }
    }
}