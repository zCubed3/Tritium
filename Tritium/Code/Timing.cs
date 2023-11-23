using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Tritium
{
    public class TimedAction
    {
        public Action action;
        public float duration, startTime;
        public bool repeating;

        public void Update()
        {
            if (Timing.elapsedTime < startTime + duration) return;

            action?.Invoke();

            if (!repeating)
                Timing.timedActions.Remove(this);
            else
                startTime = Timing.elapsedTime;
        }
    }

    public static class Timing
    {
        public static float deltaTime;
        public static float unscaledDeltaTime;

        public static Vector4 sinTime;
        public static Vector4 cosTime;

        /// <summary>
        /// Amount of real time since the start of the game (this value could potentially overflow!)
        /// </summary>
        public static float gameTime;

        /// <summary>
        /// Amount of virtual time (slowmo influenced) since the last reset (usually per scene change)
        /// </summary>
        public static float elapsedTime;

        /// <summary>
        /// Unscaled of time since the last reset (usually per scene change)
        /// </summary>
        public static float unscaledElapsedTime;

        public static float timeScale = 1F;

        public static List<TimedAction> timedActions = new List<TimedAction>();

        public static void CreateTimedAction(Action action, float waitTime, bool repeating = false)
        {
            timedActions.Add(new TimedAction() { action = action, duration = waitTime, startTime = elapsedTime, repeating = repeating });
        }

        public static void Update(GameTime time)
        {
            timedActions.ForEach(action => action.Update());

            unscaledDeltaTime = (float)time.ElapsedGameTime.TotalSeconds;
            deltaTime = unscaledDeltaTime * timeScale;

            unscaledElapsedTime += unscaledDeltaTime;
            elapsedTime += deltaTime;

            sinTime = new Vector4(MathF.Sin(elapsedTime), MathF.Sin(elapsedTime * 2), MathF.Sin(elapsedTime * 4), MathF.Sin(elapsedTime * 8));
            cosTime = new Vector4(MathF.Cos(elapsedTime), MathF.Cos(elapsedTime * 2), MathF.Cos(elapsedTime * 4), MathF.Cos(elapsedTime * 8));
        }

        public static void Reset()
        {
            elapsedTime = 0;
            unscaledElapsedTime = 0;
        }
    }
}