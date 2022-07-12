using System.Collections;
using System.Collections.Generic;
using Custom;
using Instances;
using UnityEngine;
using System.IO;
using Cinemachine;

public class KnowledgeManager : Singleton<KnowledgeManager>
{
	public string rootDocument = "root";
	public List<Task> Tasks;
	public List<Query> Queries;

	public void ReadKnowledgeAndSetUp()
	{
		var jsonContent = Resources.Load<TextAsset>("Knowledge/" + rootDocument);
		var itemsData = JSON.Parse(jsonContent.ToString());
		var tasksNode = itemsData[0]["tasks"];

		Tasks = CreateTasks(tasksNode);
	}

	public void ReadQueriesAndSetUp()
	{
		Queries = new List<Query>();
		var directoryInfo = new DirectoryInfo("Assets/Resources/Queries");
		var files = directoryInfo.GetFiles();

		foreach (var file in files)
		{
			if (!file.Name.EndsWith(".json")) continue;

			var basename = file.Name.Split('.')[0];
			var jsonContent = Resources.Load<TextAsset>("Queries/" + basename);
			var queryData = JSON.Parse(jsonContent.ToString());

			var programs = new List<string>();

			foreach (var value in queryData["programs"].Values) programs.Add(value[1]);

			Queries.Add(new Query(
				file.Name,
				queryData["query"],
				programs,
				queryData["reply"],
				queryData["context"]["taskId"],
				queryData["context"]["subtaskId"],
				queryData["context"]["instructionOrder"]
			));
		}
	}

	private List<Task> CreateTasks(JSONNode node)
	{
		var tasks = new List<Task>();
		for (var i = 0; i < node.Count; i++)
		{
			string taskId = node[i]["task_id"];
			string title = node[i]["title"];
			var figures = new List<Figure>();
			var subtasks = new List<Subtask>();

			if (node[i].HasKey("figures")) figures = CreateFigures(node[i]["figures"]);

			if (node[i].HasKey("subtasks")) subtasks = CreateSubtasks(node[i]["subtasks"]);

			tasks.Add(new Task(
				taskId,
				title,
				figures,
				subtasks
			));
		}

		return tasks;
	}

	private static List<Figure> CreateFigures(JSONNode node)
	{
		var figures = new List<Figure>();
		for (var i = 0; i < node.Count; i++)
		{
			string title = node[i]["title"];
			var figureItems = new Dictionary<string, string>();

			foreach (var item in node[i]["figure_items"].Keys) figureItems.Add(item, node[i]["figure_items"][item]);

			figures.Add(new Figure(
				title,
				figureItems
			));
		}

		return figures;
	}

	private static List<Subtask> CreateSubtasks(JSONNode node)
	{
		var subtasks = new List<Subtask>();
		for (var i = 0; i < node.Count; i++)
		{
			string subtaskId = node[i]["subtask_id"];
			string content = node[i]["content"];
			string figure = node[i]["figure"];
			var instructions = new List<Instruction>();

			if (node[i].HasKey("instructions")) instructions = CreateInstructions(node[i]["instructions"]);

			subtasks.Add(new Subtask(
				subtaskId,
				content,
				instructions,
				figure
			));
		}

		return subtasks;
	}

	private static List<Instruction> CreateInstructions(JSONNode node)
	{
		var instructions = new List<Instruction>();
		for (var i = 0; i < node.Count; i++)
		{
			string order = node[i]["order"];
			string content = node[i]["content"];
			var actions = new List<Action>();

			if (node[i].HasKey("actions")) actions = CreateActions(node[i]["actions"]);

			instructions.Add(new Instruction(
				order,
				content,
				actions
			));
		}

		return instructions;
	}

	private static List<Action> CreateActions(JSONNode node)
	{
		var actions = new List<Action>();

		for (var i = 0; i < node.Count; i++)
		{
			var action = node[i];

			foreach (var operation in action.Keys)
				actions.Add(new Action(
					operation,
					new List<string> {action[operation][0].ToString(), action[operation][1].ToString()}
				));
		}

		return actions;
	}

	public void ExecuteSubtask()
	{
		var instructions = ContextManager.Instance.CurrentSubtask.Instructions;
		var primitives = new List<IEnumerator>();

		primitives.Add(PrimitiveManager.SimplePrimitive(() => { UIManager.Instance.DisableAllButtons(); }));

		foreach (var instruction in instructions)
		{
			primitives.Add(PrimitiveManager.SimplePrimitive(() => { ContextManager.Instance.SetCurrentInstruction(instruction); }));
			primitives.Add(PrimitiveManager.SimplePrimitive(() => { UIManager.Instance.UpdateUI(); }));

			var actions = instruction.Actions;

			foreach (var action in actions)
				if (action.Operation == "detach")
				{
					var attachingObj = AssetManager.Instance.FindObjectInFigure(AssetManager.FigureType.Current, "[" + action.Components[0] + "]");
					var referenceObj = AssetManager.Instance.FindObjectInFigure(AssetManager.FigureType.Current, "[" + action.Components[1] + "]");

					var objectMeta = attachingObj.GetComponent<ObjectMeta>();

					var text = "";
					const string delimiter = " and ";

					text += objectMeta.attachType switch
					{
						ObjectMeta.AttachTypes.SmoothInstall => "Smooth Uninstall ",
						ObjectMeta.AttachTypes.StepInstall => "Step Uninstall ",
						ObjectMeta.AttachTypes.SmoothScrew => "Smooth Unscrew ",
						ObjectMeta.AttachTypes.StepScrew => "Step Unscrew ",
						_ => "Smooth Uninstall "
					};

					text += attachingObj.name + delimiter + referenceObj.name;
					primitives.Add(PrimitiveManager.SimplePrimitive(() => { UIManager.Instance.UpdateActionsList("- " + text); }));
					primitives.Add(PrimitiveManager.SimplePrimitive(() => { UIManager.Instance.UpdateReply(text); }));


					primitives.Add(PrimitiveManager.SimplePrimitive(() =>
					{
						var virtualCamera = GameObject.FindWithTag("VirtualCamera");

						// virtualCamera.GetComponent<CinemachineVirtualCamera>().enabled = true;
						virtualCamera.GetComponent<CinemachineVirtualCamera>().m_LookAt = attachingObj.transform;
						virtualCamera.GetComponent<CinemachineVirtualCamera>().m_Follow = attachingObj.transform;
					}));

					primitives.Add(Robot.Instance.Wait(2f));

					var rotationAxis = objectMeta.attachRotationAxis;

					var attachRotationVector = rotationAxis switch
					{
						ObjectMeta.RotationAxisEnum.X => Vector3.right,
						ObjectMeta.RotationAxisEnum.NegX => Vector3.left,
						ObjectMeta.RotationAxisEnum.Y => Vector3.up,
						ObjectMeta.RotationAxisEnum.NegY => Vector3.down,
						ObjectMeta.RotationAxisEnum.Z => Vector3.forward,
						ObjectMeta.RotationAxisEnum.NegZ => Vector3.back,
						_ => Vector3.forward
					};

					switch (objectMeta.attachType)
					{
						case ObjectMeta.AttachTypes.SmoothInstall:
							primitives.AddRange(PrimitiveManager.SmoothInstall(attachingObj, referenceObj));
							break;
						case ObjectMeta.AttachTypes.StepInstall:
							primitives.AddRange(PrimitiveManager.StepInstall(attachingObj, referenceObj));
							break;
						case ObjectMeta.AttachTypes.SmoothScrew:
							primitives.AddRange(PrimitiveManager.SmoothScrew(attachingObj, referenceObj, attachRotationVector));
							break;
						case ObjectMeta.AttachTypes.StepScrew:
							primitives.AddRange(PrimitiveManager.StepScrew(attachingObj, referenceObj, attachRotationVector));
							break;
						default:
							primitives.AddRange(PrimitiveManager.SmoothInstall(attachingObj, referenceObj));
							break;
					}

					primitives.Add(PrimitiveManager.SimplePrimitive(() =>
					{
						var virtualCamera = GameObject.FindWithTag("VirtualCamera");

						// virtualCamera.GetComponent<CinemachineVirtualCamera>().m_LookAt = null;
						// virtualCamera.GetComponent<CinemachineVirtualCamera>().m_Follow = null;
						// virtualCamera.GetComponent<CinemachineVirtualCamera>().enabled = false;
					}));

					primitives.Add(Robot.Instance.Wait(0.5f));
					primitives.Add(PrimitiveManager.MakeObjectTransparent(attachingObj));
					// primitives.AddRange(PrimitiveManager.CreateRfmToScatteredMovePrimitives(attachingObj));
					// primitives.AddRange(PrimitiveManager.CreateRotatePrimitives(attachingObj));
				}
		}

		primitives.Add(PrimitiveManager.SimplePrimitive(() => { UIManager.Instance.EnableAllButtons(); }));
		primitives.Add(PrimitiveManager.SimplePrimitive(() => Instance.SetCurrentSubtaskCompleted()));

		StartCoroutine(Sequence(primitives));
	}

	public void GoToNextSubtask()
	{
		var tasks = Instance.Tasks;
		var currentTask = ContextManager.Instance.CurrentTask;
		var currentSubtask = ContextManager.Instance.CurrentSubtask;

		for (var i = 0; i < tasks.Count; i++)
			if (tasks[i].TaskId == currentTask.TaskId)
				for (var j = 0; j < tasks[i].Subtasks.Count; j++)
					if (tasks[i].Subtasks[j].SubtaskId == currentSubtask.SubtaskId)
					{
						if (j < tasks[i].Subtasks.Count - 1)
						{
							ContextManager.Instance.SetCurrentSubtask(tasks[i].Subtasks[j + 1]);
							return;
						}

						if (i < tasks.Count - 1)
						{
							ContextManager.Instance.SetCurrentTask(tasks[i + 1]);
							ContextManager.Instance.SetCurrentSubtask(tasks[i + 1].Subtasks[0]);
							return;
						}

						ContextManager.Instance.ResetCurrentTask();
						return;
					}
	}

	public void SetCurrentSubtaskCompleted()
	{
		var currentTask = ContextManager.Instance.CurrentTask;
		var currentSubtask = ContextManager.Instance.CurrentSubtask;

		for (var i = 0; i < Tasks.Count; i++)
			if (Tasks[i].TaskId == currentTask.TaskId)
				for (var j = 0; j < Tasks[i].Subtasks.Count; j++)
					if (Tasks[i].Subtasks[j].SubtaskId == currentSubtask.SubtaskId)
					{
						Tasks[i].Subtasks[j].isCompleted = true;
						break;
					}
	}

	public IEnumerator Sequence(List<IEnumerator> sequence)
	{
		foreach (var coroutine in sequence) yield return StartCoroutine(coroutine);

		yield return null;
	}
}