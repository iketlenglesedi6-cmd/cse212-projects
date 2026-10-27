public static class ArraySelector
{
    public static void Run()
    {
        var l1 = new[] { 1, 2, 3, 4, 5 };
        var l2 = new[] { 2, 4, 6, 8, 10};
        var select = new[] { 1, 1, 1, 2, 2, 1, 2, 2, 2, 1};
        var intResult = ListSelector(l1, l2, select);
        Console.WriteLine("<int[]>{" + string.Join(", ", intResult) + "}"); // <int[]>{1, 2, 3, 2, 4, 4, 6, 8, 10, 5}
    }

    private static int[] ListSelector(int[] list1, int[] list2, int[] select)
    {
        // 1. Make a result array with one space for each selector value.
        // 2. Start at position 0 in both input arrays.
        // 3. For each selector value, take the next item from list1 for 1 or list2 for 2.
        // 4. Move forward only in the input array we just used.
        // 5. Return the result once all the selector values have been used.
        int[] results = new int[select.Length];
        int array1Index = 0;
        int array2Index = 0;

        for (int index = 0; index < select.Length; index++)
        {
            if (select[index] == 1)
            {
                results[index] = list1[array1Index];
                array1Index++;
            }
            else
            {
                results[index] = list2[array2Index];
                array2Index++;
            }
        }

        return results;
    }
}
