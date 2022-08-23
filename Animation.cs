using Microsoft.Xna.Framework;

namespace RogueClone
{
	internal class Animation
	{
		private readonly int startOffset;
		private readonly int frameCount;
		private int currentFrame = 0;
		private float timer = 0f;
		private readonly float interval;
		private readonly int size;

		public Animation(int startOffset, int frameCount, float interval, int size)
		{
			this.startOffset = startOffset;
			this.frameCount = frameCount;
			this.interval = interval;
			this.size = size;
		}

		public void Update(float time)
		{
			timer += time;
			if (timer >= interval)
			{
				timer = 0f;
				currentFrame = (currentFrame + 1) % frameCount;
			}
		}

		public Rectangle GetSourceRectangle()
		{
			return new Rectangle(startOffset + size * currentFrame, 0, size, size);
		}

	}
}
