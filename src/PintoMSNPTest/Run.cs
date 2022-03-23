using System;
using PintoMSNPTest;

static class Run 
{
	static void Main(string[] args)
	{
		(MainClass.Instance = new MainClass()).Start();
	}
}