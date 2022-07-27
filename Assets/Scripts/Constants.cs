using System.Collections.Generic;

public static class Constants
{
	public const char ArgsSeparator = '#';
	public const string FigureRegex = @"(\d+-)+[A-Z]{1}";
	public const string ObjectRegex = @"[0-9]+";
	public const string NumberRegex = @"(([0-9]*[.])?[0-9]+)";

	public const string ValidActions = "yes";
	public const string InvalidActions = "no";

	public enum FigureType
	{
		Current,
		Ifm,
		Rfm,
		Reference,
		Scattered
	}

	public enum TaskType
	{
		Installation,
		Removal,
		Other
	}

	public delegate void FunctionDelegate();

	public const string ModelPrefabFolder = "ModelPrefabs";
}