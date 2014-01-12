﻿using UnityEngine;
using System.Collections;
using UnityEditor;

public class PMDLoaderWindow : EditorWindow {
	Object pmdFile = null;
	bool rigidFlag = true;
	MMD.PMXConverter.AnimationType animation_type = MMD.PMXConverter.AnimationType.LegacyAnimation;
	MMD.PMDConverter.ShaderType shader_type = MMD.PMDConverter.ShaderType.MMDShader;

	bool use_ik = true;
	float scale = 0.085f;
	bool is_pmx_base_import = false;

	[MenuItem("MMD for Unity/PMD Loader")]
	static void Init() {
		var window = (PMDLoaderWindow)EditorWindow.GetWindow<PMDLoaderWindow>(true, "PMDLoader");
		window.Show();
	}

	public PMDLoaderWindow()
	{
		// デフォルトコンフィグ
		var config = MMD.Config.LoadAndCreate();
		shader_type = config.pmd_config.shader_type;
		rigidFlag = config.pmd_config.rigidFlag;
		animation_type = config.pmd_config.animation_type;
		use_ik = config.pmd_config.use_ik;
		is_pmx_base_import = config.pmd_config.is_pmx_base_import;
	}
	
	void OnGUI() {
		pmdFile = EditorGUILayout.ObjectField("PMD File" , pmdFile, typeof(Object), false);
		
		// シェーダの種類
		shader_type = (MMD.PMDConverter.ShaderType)EditorGUILayout.EnumPopup("Shader Type", shader_type);

		// 剛体を入れるかどうか
		rigidFlag = EditorGUILayout.Toggle("Rigidbody", rigidFlag);

		// アニメーションタイプ
		animation_type = (MMD.PMXConverter.AnimationType)EditorGUILayout.EnumPopup("Animation Type", animation_type);

		// IKを使うかどうか
		use_ik = EditorGUILayout.Toggle("Use IK", use_ik);

		// スケール
		scale = EditorGUILayout.Slider("Scale", scale, 0.001f, 1.0f);
		EditorGUILayout.BeginHorizontal();
		{
			EditorGUILayout.PrefixLabel(" ");
			if (GUILayout.Button("Original", EditorStyles.miniButtonLeft)) {
				scale = 0.085f;
			}
			if (GUILayout.Button("1.0", EditorStyles.miniButtonRight)) {
				scale = 1.0f;
			}
		}
		EditorGUILayout.EndHorizontal();

		// PMX Baseでインポートするかどうか
		is_pmx_base_import = EditorGUILayout.Toggle("Use PMX Base Import", is_pmx_base_import);
		
		if (pmdFile != null) {
			if (GUILayout.Button("Convert")) {
				LoadModel();
				pmdFile = null;		// 読み終わったので空にする 
			}
		} else {
			EditorGUILayout.LabelField("Missing", "Select PMD File");
		}
	}

	void LoadModel() {
		string file_path = AssetDatabase.GetAssetPath(pmdFile);
		MMD.ModelAgent model_agent = new MMD.ModelAgent(file_path);
		model_agent.CreatePrefab(shader_type, rigidFlag, animation_type, use_ik, scale, is_pmx_base_import);
		
		// 読み込み完了メッセージ
		var window = LoadedWindow.Init();
		window.Text = string.Format(
			"----- model name -----\n{0}\n\n----- comment -----\n{1}",
			model_agent.name,
			model_agent.comment
		);
		window.Show();
	}
}
