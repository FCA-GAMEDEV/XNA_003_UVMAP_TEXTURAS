using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AulaXNA3D003
{
    public class _Quad
    {
        Game game;
        GraphicsDevice device;
        Matrix world;
        VertexPositionTexture[] verts;
        VertexBuffer buffer;
        BasicEffect effect;
        Texture2D texture;

        Texture2D auxTexture;
        Color[] colors;
        Random rand;

        public _Quad(GraphicsDevice device, Game game, string textureName)
        {
            this.game = game;
            this.device = device;
            this.world = Matrix.Identity;

            this.verts = new VertexPositionTexture[]
            {
                new VertexPositionTexture(new Vector3(-1, 1,0),Vector2.Zero),  //v0
                new VertexPositionTexture(new Vector3( 1, 1,0),Vector2.UnitX), //v1
                new VertexPositionTexture(new Vector3(-1,-1,0),Vector2.UnitY), //v2

                new VertexPositionTexture(new Vector3( 1, 1,0),Vector2.UnitX), //v3
                new VertexPositionTexture(new Vector3( 1,-1,0),Vector2.One),   //v4
                new VertexPositionTexture(new Vector3(-1,-1,0),Vector2.UnitY), //v5
            };

            this.buffer = new VertexBuffer(this.device,
                                           typeof(VertexPositionTexture),
                                           this.verts.Length,
                                           BufferUsage.None);
            this.buffer.SetData<VertexPositionTexture>(this.verts);

            this.effect = new BasicEffect(this.device);

            this.texture = this.game.Content.Load<Texture2D>(textureName);

            rand = new Random();
            this.colors = new Color[this.texture.Width * this.texture.Height];
            this.texture.GetData<Color>(colors);

            for (int i = 0; i < this.texture.Height; i++)
            {
                int red   = rand.Next(256);
                int green = rand.Next(256);
                int blue  = rand.Next(256);

                for (int j = 0; j < this.texture.Width; j++)
                {
                    int index = i * this.texture.Width + j;

                    colors[index] = new Color((red   + colors[index].R) / 2,
                                              (green + colors[index].G) / 2,
                                              (blue  + colors[index].B) / 2);
                }
            }

            this.auxTexture = new Texture2D(device, this.texture.Width, this.texture.Height);
            this.auxTexture.SetData<Color>(colors);
            this.texture = this.auxTexture;
        }

        public void Update(GameTime gameTime)
        {
            this.auxTexture = new Texture2D(device, this.texture.Width, this.texture.Height);

            for (int i = 0; i < this.texture.Height; i++)
            {
                for (int j = 0; j < this.texture.Width; j++)
                {
                    int index = i * this.texture.Width + j;
                    int nextIndex = i * this.texture.Width + ((j + 1) == this.texture.Width ? 0 : (j + 1));
                    colors[index] = colors[nextIndex];
                }
            }

            this.auxTexture.SetData<Color>(colors);
            this.texture = this.auxTexture;
        }

        public virtual void Draw(_Camera camera)
        {
            this.device.SetVertexBuffer(this.buffer);

            this.effect.World = this.world;
            this.effect.View = camera.GetView();
            this.effect.Projection = camera.GetProjection();
            this.effect.TextureEnabled = true;
            this.effect.Texture = this.texture;

            foreach (EffectPass pass in this.effect.CurrentTechnique.Passes)
            {
                pass.Apply();

                this.device.DrawUserPrimitives<VertexPositionTexture>(PrimitiveType.TriangleList,
                                                                    this.verts, 0, 2);
            }
        }
    }
}
