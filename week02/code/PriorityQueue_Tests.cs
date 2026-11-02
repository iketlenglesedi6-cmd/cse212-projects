using Microsoft.VisualStudio.TestTools.UnitTesting;

// TODO Problem 2 - Write and run test cases and fix the code to match requirements.

[TestClass]
public class PriorityQueueTests
{
    [TestMethod]
    // Scenario: Add items with different priorities, including the highest priority at the back.
    // Expected Result: Each item is removed once, from highest to lowest priority.
    // Defect(s) Found: Medium came out before High because the search skipped the last item.
    // Dequeue also left the returned item in the queue.
    // Result: Passed after checking every item and removing the selected item.
    public void TestPriorityQueue_1()
    {
        var priorityQueue = new PriorityQueue();
        priorityQueue.Enqueue("Low", 1);
        priorityQueue.Enqueue("Medium", 3);
        priorityQueue.Enqueue("High", 5);

        Assert.AreEqual("High", priorityQueue.Dequeue());
        Assert.AreEqual("Medium", priorityQueue.Dequeue());
        Assert.AreEqual("Low", priorityQueue.Dequeue());
        Assert.ThrowsException<InvalidOperationException>(() => priorityQueue.Dequeue());
    }

    [TestMethod]
    // Scenario: Add two items with the same highest priority and a lower-priority item between them.
    // Expected Result: The first high-priority item leaves first, then the second, then the low item.
    // Defect(s) Found: First high was returned twice because it was never removed. After
    // fixing removal and the search, Second high came out first because ties replaced the earlier item.
    // Result: Passed after keeping the first item when priorities are equal.
    public void TestPriorityQueue_2()
    {
        var priorityQueue = new PriorityQueue();
        priorityQueue.Enqueue("First high", 5);
        priorityQueue.Enqueue("Low", 1);
        priorityQueue.Enqueue("Second high", 5);

        Assert.AreEqual("First high", priorityQueue.Dequeue());
        Assert.AreEqual("Second high", priorityQueue.Dequeue());
        Assert.AreEqual("Low", priorityQueue.Dequeue());
        Assert.ThrowsException<InvalidOperationException>(() => priorityQueue.Dequeue());
    }

    // Add more test cases as needed below.

    [TestMethod]
    // Scenario: Try to remove an item from a new, empty queue.
    // Expected Result: InvalidOperationException with the message "The queue is empty."
    // Defect(s) Found: None. The exception type and message were already correct.
    // Result: Passed before and after the fixes.
    public void TestPriorityQueue_Empty()
    {
        var priorityQueue = new PriorityQueue();

        var exception = Assert.ThrowsException<InvalidOperationException>(() => priorityQueue.Dequeue());

        Assert.AreEqual("The queue is empty.", exception.Message);
    }

    [TestMethod]
    // Scenario: Add one item, remove it, then try to remove another item.
    // Expected Result: The item leaves once, then the empty-queue exception is thrown.
    // Defect(s) Found: A second call returned the same item instead of throwing an exception.
    // Result: Passed after Dequeue removed the item from the queue.
    public void TestPriorityQueue_SingleItem()
    {
        var priorityQueue = new PriorityQueue();
        priorityQueue.Enqueue("Only item", 2);

        Assert.AreEqual("Only item", priorityQueue.Dequeue());

        var exception = Assert.ThrowsException<InvalidOperationException>(() => priorityQueue.Dequeue());
        Assert.AreEqual("The queue is empty.", exception.Message);
    }

    [TestMethod]
    // Scenario: Add items with negative and zero priorities.
    // Expected Result: Zero leaves first, followed by -2 and then -5.
    // Defect(s) Found: Middle came out before Zero because the search skipped the last item.
    // Result: Passed after fixing the search and removing each returned item.
    public void TestPriorityQueue_NonPositivePriorities()
    {
        var priorityQueue = new PriorityQueue();
        priorityQueue.Enqueue("Lowest", -5);
        priorityQueue.Enqueue("Middle", -2);
        priorityQueue.Enqueue("Zero", 0);

        Assert.AreEqual("Zero", priorityQueue.Dequeue());
        Assert.AreEqual("Middle", priorityQueue.Dequeue());
        Assert.AreEqual("Lowest", priorityQueue.Dequeue());
        Assert.ThrowsException<InvalidOperationException>(() => priorityQueue.Dequeue());
    }

    [TestMethod]
    // Scenario: Remove one of two equal-priority items, then add another with the same priority.
    // Expected Result: The waiting item leaves before the newly added item.
    // Defect(s) Found: Items stayed in the queue after being returned. Once removal and the
    // search were fixed, equal priorities still picked the later item first.
    // Result: Passed after fixing removal and keeping equal-priority items in arrival order.
    public void TestPriorityQueue_AddAfterDequeue()
    {
        var priorityQueue = new PriorityQueue();
        priorityQueue.Enqueue("First", 4);
        priorityQueue.Enqueue("Second", 4);

        Assert.AreEqual("First", priorityQueue.Dequeue());
        priorityQueue.Enqueue("Third", 4);

        Assert.AreEqual("Second", priorityQueue.Dequeue());
        Assert.AreEqual("Third", priorityQueue.Dequeue());
        Assert.ThrowsException<InvalidOperationException>(() => priorityQueue.Dequeue());
    }
}