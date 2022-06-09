using UnityEngine;

public static class TestCustomMethod
{
	public static void TestMethod()
	{
		System.Diagnostics.Process.Start("cmd.exe", "echo Custom method has been invoked");
		Debug.Log("My custom message!!!");
	}
}