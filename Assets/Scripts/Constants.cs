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
		Reference
	}

	public enum TaskType
	{
		Installation,
		Removal,
		Other
	}

	public delegate void FunctionDelegate();

	public const string ModelPrefabFolder = "ModelPrefabs";
	public const string QueriesDirectory = "Queries";
}

public static class Tags
{
	public const string Figure = "Figure";
	public const string Object = "Object";
	public const string ReferenceObject = "ReferenceObject";
	public const string CloneObject = "CloneObject";

	public const string Grid = "Grid";

	public const string KnowledgeContent = "KnowledgeContent";
	public const string QueriesContent = "QueriesContent";

	public const string KnowledgeTaskUI = "KnowledgeTaskUI";
	public const string KnowledgeSubtaskUI = "KnowledgeSubtaskUI";
	public const string KnowledgeInstructionUI = "KnowledgeInstructionUI";
	public const string KnowledgeActionsUI = "KnowledgeActionsUI";
	public const string BasicOperationsUI = "BasicOperationsUI";

	public const string QueryText = "QueryText";
	public const string ReplyUI = "ReplyUI";
	public const string TaskOptionUI = "TaskOptionUI";
	public const string SubtaskOptionUI = "SubtaskOptionUI";
	public const string QueryOptionUI = "QueryOptionUI";

	public const string HomeButton = "HomeButton";
	public const string PlayButton = "PlayButton";
	public const string NextButton = "NextButton";
	public const string PreviousButton = "PreviousButton";
	public const string ResetButton = "ResetButton";
	public const string QueryPlayButton = "QueryPlayButton";

	public const string VirtualCamera = "VirtualCamera";
	public const string FigureFocusVirtualCamera = "FigureFocusVirtualCamera";
	public const string ObjectFocusVirtualCamera = "ObjectFocusVirtualCamera";
}