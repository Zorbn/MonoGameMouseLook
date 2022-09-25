using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.IO;
using System;

namespace MonogameDelta;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    private VertexPositionColor[] _primitiveList;
    private BasicEffect _basicEffect;
    private float _lookY, _lookX;
    private Vector2 _center;
    private readonly float _minLookRad = MathHelper.ToRadians(-89f), _maxLookRad = MathHelper.ToRadians(89f);
    private const float MouseAccel = 0.002f;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        IsMouseVisible = false;

        Mouse.SetPosition(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2);

        _center = new Vector2(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2);

        _basicEffect = new BasicEffect(GraphicsDevice);
        _basicEffect.World = Matrix.Identity + Matrix.CreateTranslation(0f, 0f, 0f);
        _basicEffect.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45f),
            GraphicsDevice.Viewport.Width / (float)GraphicsDevice.Viewport.Height, 1f, 100f);
        _basicEffect.VertexColorEnabled = true;

        _primitiveList = new[]
        {
            new VertexPositionColor(new Vector3(0f, 0f, 0f), new Color(1.0f, 0.0f, 0.0f, 1.0f)),
            new VertexPositionColor(new Vector3(1f, 0f, 0f), new Color(0.0f, 1.0f, 0.0f, 1.0f)),
            new VertexPositionColor(new Vector3(1f, 1f, 0f), new Color(0.0f, 0.0f, 1.0f, 1.0f))
        };

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        Vector2 delta = Mouse.GetState().Position.ToVector2() - _center;
        Mouse.SetPosition(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2);

        _lookX -= delta.Y * MouseAccel;
        _lookY -= delta.X * MouseAccel;

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        GraphicsDevice.RasterizerState = RasterizerState.CullNone;

        Vector3 pos = new(0f, 0f, -3f);

        Matrix yRot = Matrix.CreateRotationY(MathHelper.WrapAngle(_lookY));
        Matrix xRot = Matrix.CreateRotationX(Math.Clamp(-_lookX, _minLookRad, _maxLookRad));

        Vector3 lookAt = pos + Vector3.Transform(Vector3.UnitZ, xRot * yRot);

        Matrix view = Matrix.CreateLookAt(pos, lookAt, Vector3.Up);

        _basicEffect.View = view;

        foreach (EffectPass currentTechniquePass in _basicEffect.CurrentTechnique.Passes)
        {
            currentTechniquePass.Apply();
            GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, _primitiveList, 0, _primitiveList.Length / 3);
        }

        base.Draw(gameTime);
    }
}
