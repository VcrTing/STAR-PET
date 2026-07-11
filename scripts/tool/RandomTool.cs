using Godot;
using System;

public static class RandomTool
{
	private static Random _random = new Random();

	/// <summary>
	/// 返回 [min, max) 范围内的随机浮点数
	/// </summary>
	public static float Range(float min, float max)
	{
		return (float)(_random.NextDouble() * (max - min) + min);
	}

	/// <summary>
	/// 返回 [min, max] 范围内的随机整数
	/// </summary>
	public static int Range(int min, int max)
	{
		return _random.Next(min, max + 1);
	}
}