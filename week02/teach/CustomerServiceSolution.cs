public static class CustomerServiceSolution
{
	public static void Main()
	{
		Console.WriteLine("\n======================\nCustomer Service Solution\n======================");
		// Comparison: both versions check for a full queue with >=, check for an empty queue,
		// and save the next customer's details before removing them. Our tests also check negative sizes.
		CustomerService.Run();
	}
}