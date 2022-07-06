﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RootManager : MonoBehaviour
{
	public SceneManager sceneManager;
	public KnowledgeManager knowledgeManager;
	public UIManager uiManager;
	public ContextManager contextManager;
	public AssetManager assetManager;

	public static Robot Robot;

	public static RootManager Instance { get; private set; }

	private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Destroy(this);
		}
		else
		{
			Instance = this;
		}
	}

	private void Start()
	{
		Robot = new Robot();
	}
	
	public IEnumerator Sequence(List<IEnumerator> sequence)
	{
		foreach (var coroutine in sequence)
		{
			yield return StartCoroutine(coroutine);
		}

		yield return null;
	}
}