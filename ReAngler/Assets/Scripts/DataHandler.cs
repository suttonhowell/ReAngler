using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;

public class DataHandler : MonoBehaviour
{
	static string SPLIT_RE = @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))";
	static string LINE_SPLIT_RE = @"\r\n|\n\r|\n|\r";
	static char[] TRIM_CHARS = { '\"' };
	public TextAsset stagesImport;

	// Start is called before the first frame update
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{

	}

	public List<Dictionary<string, object>> StageReader()
	{
		List<Dictionary<string, object>> data = ReadInStages();
		if (data == null) throw new Exception("Stages were not processed correctly.");
		return data;
	}

	public static void SaveResults(ArrayList userScores)
	{
		string filePath = Application.dataPath + "/Resources/Results.csv";

		StreamWriter writer = new StreamWriter(filePath);
		writer.WriteLine("round,time,audioEnabled,hapticsEnabled,userTime");

		foreach (Round round in userScores)
		{
			int audioEnabled = round.currStage.audioEnabled ? 1 : 0;
			int hapticsEnabled = round.currStage.hapticsEnabled ? 1 : 0;

			string roundData = round.currStage.round + "," + round.currStage.time
					+ "," + audioEnabled + "," + hapticsEnabled + "," + round.userTime;
			writer.WriteLine(roundData);
		}

		writer.Flush();
		writer.Close();
	}

	private List<Dictionary<string, object>> ReadInStages()
	{
		var list = new List<Dictionary<string, object>>();
		if (stagesImport == null) throw new Exception("Stage information file could not be found.");

		var lines = Regex.Split(stagesImport.text, LINE_SPLIT_RE);

		if (lines.Length <= 1) return list;

		var header = Regex.Split(lines[0], SPLIT_RE);
		for (var i = 1; i < lines.Length; i++)
		{

			var values = Regex.Split(lines[i], SPLIT_RE);
			if (values.Length == 0 || values[0] == "") continue;

			var entry = new Dictionary<string, object>();
			for (var j = 0; j < header.Length && j < values.Length; j++)
			{
				string value = values[j];
				value = value.TrimStart(TRIM_CHARS).TrimEnd(TRIM_CHARS).Replace("\\", "");
				object finalvalue = value;
				int n;
				float f;
				if (int.TryParse(value, out n))
				{
					finalvalue = n;
				}
				else if (float.TryParse(value, out f))
				{
					finalvalue = f;
				}
				entry[header[j]] = finalvalue;
			}
			list.Add(entry);
		}
		return list;
	}
}
