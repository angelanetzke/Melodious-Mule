using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MelodiousMule
{
	internal abstract class AbstractScene
	{
		public abstract void LoadContent();
		public abstract MelodiousMule.GameState Update(GameTime gameTime, MouseState mouseState, KeyboardState keyState);
		public abstract void Draw(SpriteBatch batch);
	}
}
