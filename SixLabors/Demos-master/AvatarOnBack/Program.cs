using ImageSharp;
using ImageSharp.Drawing.Brushes;
using SixLabors.Fonts;
using System;
using System.IO;

namespace AvatarOnBack
{
    class Program
    {
        private static FontCollection fontCollection;
        private static Font iconFont;
        private static Font textFont;
        private static Font titleFont;

        static void Main(string[] args)
        {
            fontCollection = new FontCollection();
            iconFont = fontCollection.Install("fonts/fontawesome-webfont.ttf");
            titleFont = fontCollection.Install("fonts/Lato-Bold.ttf");
            textFont = fontCollection.Install("fonts/Lato-Regular.ttf");

            var avatars = Directory.EnumerateFiles("Avatars");
            var backgrounds = Directory.EnumerateFiles("Backgrounds");
            Directory.CreateDirectory("output");

            foreach (var format in new[] { "png", "jpg" })
            {
                int counter = 0;
                foreach (var a in avatars)
                {
                    foreach (var b in backgrounds)
                    {
                        counter++;

                        GenerateImage(a, b, "\uf006", "Serenity", 2, 142, 900004, $"output/result-{counter}.{format}");
                    }
                }
            }
        }


        private static void GenerateImage(string avatarUrl, string backgroundUrl, string icon, string name, int rank, int level, int ep, string outputPath)
        {
            using (var output = new Image<Rgba32>(400, 222))
            {
                DrawBackground(backgroundUrl, output);

                DrawStats(rank, level, ep, output);

                DrawTitle(icon, name, output);

                DrawAvatar(avatarUrl, output);

                output.Save(outputPath);
            } // dispose of output to help save memory

        }

        private static void DrawBackground(string backgroundUrl, Image<Rgba32> output)
        {
            using (Image<Rgba32> background = Image.Load(backgroundUrl)) // 400x222
            {
                background.Resize(new ImageSharp.Processing.ResizeOptions
                {
                    Mode = ImageSharp.Processing.ResizeMode.Crop,
                    Size = new ImageSharp.Size(output.Width, output.Height)
                });

                //draw on the background
                output.DrawImage(background, 1, new ImageSharp.Size(400, 222), new Point(0, 0));
            } // once draw it can be disposed as its no longer needed in memory
        }

        private static void DrawAvatar(string avatarUrl, Image<Rgba32> output)
        {
            var avatarPosition = new Rectangle(42, 125, 57, 57);
            var avatarPadding = 1;

            // avatar background/border
            output.Fill(Rgba32.Gainsboro, new Rectangle(avatarPosition.X - avatarPadding, avatarPosition.Y - avatarPadding, avatarPosition.Width + (avatarPadding * 2), avatarPosition.Height + (avatarPadding * 2)));

            using (var avatar = Image.Load(avatarUrl)) 
            {
                avatar.Resize(new ImageSharp.Processing.ResizeOptions
                {
                    Mode = ImageSharp.Processing.ResizeMode.Crop,
                    Size = avatarPosition.Size
                });

                // draw the avatar image
                output.DrawImage(avatar, 1, avatarPosition.Size, avatarPosition.Location);
            }
        }

        private static void DrawStats(int rank, int level, int ep, Image<Rgba32> output)
        {
            Rgba32 statsColor = Rgba32.Gainsboro;
            statsColor.A = 210; // git it a little opacity

            // stats background
            output.Fill(statsColor, new Rectangle(56, 174, 345, 48));
            
            var font = new Font(textFont, 20, FontStyle.Regular);
            output.DrawText($"Rank: {rank}", font, Rgba32.Gray, new System.Numerics.Vector2(108, 191), new ImageSharp.Drawing.TextGraphicsOptions
            {
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Left
            });
            output.DrawText($"Level: {level}", font, Rgba32.Gray, new System.Numerics.Vector2(235, 191), new ImageSharp.Drawing.TextGraphicsOptions
            {
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center
            });
            output.DrawText($"EP: {ep}", font, Rgba32.Gray, new System.Numerics.Vector2(394, 191), new ImageSharp.Drawing.TextGraphicsOptions
            {
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Right
            });
        }

        private static void DrawTitle(string icon, string name, Image<Rgba32> output)
        {
            var titleColor = Rgba32.DimGray;
            titleColor.A = 220; // just a touch of opacity

            // title background
            output.Fill(Rgba32.DimGray, new Rectangle(100, 134, 300, 40)); //draw the background
            
            // add the icon
            output.DrawText(icon, new Font(iconFont, 22), Rgba32.White, new System.Numerics.Vector2(114, 142));

            // add the name
            output.DrawText(name, new Font(titleFont, 32, FontStyle.Bold), Rgba32.White, new System.Numerics.Vector2(140, 128));
        }
    }
}