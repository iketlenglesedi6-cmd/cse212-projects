/// <summary>
/// Maintain a Customer Service Queue.  Allows new customers to be 
/// added and allows customers to be serviced.
/// </summary>
public class CustomerService {
    public static void Run() {
        // Example code to see what's in the customer service queue:
        // var cs = new CustomerService(10);
        // Console.WriteLine(cs);

        // Test Cases
        int passed = 0;
        int failed = 0;

        // Test 1
        // Scenario: Create a queue with room for two customers and add Alice and Bob.
        // Expected Result: The size limit is two, and both customers' details stay in arrival order.
        Console.WriteLine("Test 1");
        RunTest(() => {
            var service = new CustomerService(2);
            Check(service.ToString() == "[size=0 max_size=2 => ]", "The requested size was not kept.");
            AddCustomer(service, "Alice", "A1", "Login problem");
            AddCustomer(service, "Bob", "B2", "Billing question");
            Check(service.ToString() == "[size=2 max_size=2 => Alice (A1)  : Login problem, Bob (B2)  : Billing question]",
                "Customers or their details were not added in arrival order.");
        });

        // Defect(s) Found: None. A valid size and adding customers already worked.
        // Result: Passed before and after the fixes.

        Console.WriteLine("=================");

        // Test 2
        // Scenario: Fill a queue with a limit of one, then try to add Bob.
        // Expected Result: Show the full-queue message and leave Alice as the only customer.
        Console.WriteLine("Test 2");
        RunTest(() => {
            var service = new CustomerService(1);
            AddCustomer(service, "Alice", "A1", "Login problem");
            var before = service.ToString();
            var output = AddCustomer(service, "Bob", "B2", "Billing question");
            Check(output.Contains("Maximum Number of Customers in Queue."), "The full-queue message was missing.");
            Check(service.ToString() == before, "A customer was added beyond the size limit.");
            Check(!output.Contains("Customer Name:"), "A full queue should not ask for another customer's details.");
        });

        // Defect(s) Found: The full queue accepted Bob because the check used > instead of >=.
        // Result: Failed before the fix. Passed after rejecting customers as soon as the queue is full.

        Console.WriteLine("=================");

        // Add more Test Cases As Needed Below

        // Test 3
        // Scenario: Add Alice, Bob, and Cara, then serve them one at a time.
        // Expected Result: Display and remove Alice first, then Bob, then Cara.
        Console.WriteLine("Test 3");
        RunTest(() => {
            var service = new CustomerService(3);
            AddCustomer(service, "Alice", "A1", "Login problem");
            AddCustomer(service, "Bob", "B2", "Billing question");
            AddCustomer(service, "Cara", "C3", "Password reset");
            Check(CaptureConsole(service.ServeCustomer).Trim() == "Alice (A1)  : Login problem",
                "The first customer served was not Alice.");
            Check(service.ToString() == "[size=2 max_size=3 => Bob (B2)  : Billing question, Cara (C3)  : Password reset]",
                "Serving Alice did not leave Bob and Cara waiting in order.");
            Check(CaptureConsole(service.ServeCustomer).Trim() == "Bob (B2)  : Billing question",
                "The second customer served was not Bob.");
            Check(CaptureConsole(service.ServeCustomer).Trim() == "Cara (C3)  : Password reset",
                "The last customer served was not Cara.");
            Check(service.ToString() == "[size=0 max_size=3 => ]", "The queue was not emptied.");
        });
        // Defect(s) Found: Alice was removed before her details were read, so Bob was displayed first.
        // Result: Failed before the fix. Passed after saving the first customer before removing them.

        Console.WriteLine("=================");

        // Test 4
        // Scenario: Try to serve a customer from a new, empty queue.
        // Expected Result: Display an empty-queue message without crashing or changing the queue.
        Console.WriteLine("Test 4");
        RunTest(() => {
            var service = new CustomerService(2);
            var output = CaptureConsole(service.ServeCustomer);
            Check(output.Trim() == "No Customers in Queue.", "The empty-queue message was missing.");
            Check(service.ToString() == "[size=0 max_size=2 => ]", "The empty queue changed.");
        });
        // Defect(s) Found: Removing from an empty list threw an exception instead of showing a message.
        // Result: Failed before the fix. Passed after checking whether the queue is empty.

        Console.WriteLine("=================");

        // Test 5
        // Scenario: Create a queue with a size of zero, add ten customers, then try an eleventh.
        // Expected Result: Default to ten, accept ten customers, and reject the eleventh.
        Console.WriteLine("Test 5");
        RunTest(() => CheckDefaultCapacity(0));
        // Defect(s) Found: Zero correctly defaulted to ten, but an eleventh customer was still accepted.
        // Result: Failed before the full-queue fix. Passed after changing > to >=.

        Console.WriteLine("=================");

        // Test 6
        // Scenario: Create a queue with a negative size and try to add eleven customers.
        // Expected Result: Default to ten and reject the eleventh customer.
        Console.WriteLine("Test 6");
        RunTest(() => CheckDefaultCapacity(-3));
        // Defect(s) Found: A negative size correctly defaulted to ten, but the queue accepted an eleventh customer.
        // Result: Failed before the full-queue fix. Passed after changing > to >=.

        Console.WriteLine("=================");

        // Test 7
        // Scenario: Serve the only customer, try serving again, then add a new customer.
        // Expected Result: Serve the first customer once, handle the empty queue, and reuse the space.
        Console.WriteLine("Test 7");
        RunTest(() => {
            var service = new CustomerService(1);
            AddCustomer(service, "Alice", "A1", "Login problem");
            Check(CaptureConsole(service.ServeCustomer).Trim() == "Alice (A1)  : Login problem",
                "Serving the only customer did not display her details.");
            Check(CaptureConsole(service.ServeCustomer).Trim() == "No Customers in Queue.",
                "Serving the emptied queue did not display an error message.");
            AddCustomer(service, "Bob", "B2", "Billing question");
            Check(CaptureConsole(service.ServeCustomer).Trim() == "Bob (B2)  : Billing question",
                "The space could not be reused for Bob.");
            Check(service.ToString() == "[size=0 max_size=1 => ]", "Bob was not removed after being served.");
        });
        // Defect(s) Found: Serving the only customer removed her and then tried to read from the empty list.
        // Result: Failed before the fixes. Passed after saving the customer first and handling empty queues.

        Console.WriteLine($"Customer Service tests: {passed} passed, {failed} failed.");
        if (failed > 0)
            throw new InvalidOperationException("Some Customer Service tests failed.");

        // Keep running the other cases if one test fails, so we can see all the problems.
        void RunTest(Action test) {
            try {
                test();
                passed++;
                Console.WriteLine("PASS");
            }
            catch (Exception error) {
                failed++;
                Console.WriteLine($"FAIL: {error.Message}");
            }
        }

        void CheckDefaultCapacity(int size) {
            var service = new CustomerService(size);
            Check(service.ToString() == "[size=0 max_size=10 => ]", "An invalid size did not default to ten.");
            for (int i = 1; i <= 10; i++)
                AddCustomer(service, $"Customer {i}", $"A{i}", "Account question");
            var before = service.ToString();
            Check(before.StartsWith("[size=10 max_size=10 => "), "The queue did not accept ten customers.");
            var output = AddCustomer(service, "Extra customer", "A11", "Should not fit");
            Check(output.Contains("Maximum Number of Customers in Queue."), "The eleventh customer was not rejected.");
            Check(service.ToString() == before, "Rejecting the eleventh customer changed the queue.");
        }

        static void Check(bool condition, string message) {
            if (!condition)
                throw new InvalidOperationException(message);
        }

        static string AddCustomer(CustomerService service, string name, string accountId, string problem) {
            return CaptureConsole(service.AddNewCustomer, $"{name}\n{accountId}\n{problem}\n");
        }

        // Supply typed input and capture output for a test, then restore the normal console.
        static string CaptureConsole(Action action, string input = "") {
            var originalInput = Console.In;
            var originalOutput = Console.Out;
            using var reader = new StringReader(input);
            using var writer = new StringWriter();
            try {
                Console.SetIn(reader);
                Console.SetOut(writer);
                action();
                return writer.ToString();
            }
            finally {
                Console.SetIn(originalInput);
                Console.SetOut(originalOutput);
            }
        }
    }

    private readonly List<Customer> _queue = new();
    private readonly int _maxSize;

    public CustomerService(int maxSize) {
        if (maxSize <= 0)
            _maxSize = 10;
        else
            _maxSize = maxSize;
    }

    /// <summary>
    /// Defines a Customer record for the service queue.
    /// This is an inner class.  Its real name is CustomerService.Customer
    /// </summary>
    private class Customer {
        public Customer(string name, string accountId, string problem) {
            Name = name;
            AccountId = accountId;
            Problem = problem;
        }

        private string Name { get; }
        private string AccountId { get; }
        private string Problem { get; }

        public override string ToString() {
            return $"{Name} ({AccountId})  : {Problem}";
        }
    }

    /// <summary>
    /// Prompt the user for the customer and problem information.  Put the 
    /// new record into the queue.
    /// </summary>
    private void AddNewCustomer() {
        // Verify there is room in the service queue
        if (_queue.Count >= _maxSize) {
            Console.WriteLine("Maximum Number of Customers in Queue.");
            return;
        }

        Console.Write("Customer Name: ");
        var name = Console.ReadLine()!.Trim();
        Console.Write("Account Id: ");
        var accountId = Console.ReadLine()!.Trim();
        Console.Write("Problem: ");
        var problem = Console.ReadLine()!.Trim();

        // Create the customer object and add it to the queue
        var customer = new Customer(name, accountId, problem);
        _queue.Add(customer);
    }

    /// <summary>
    /// Dequeue the next customer and display the information.
    /// </summary>
    private void ServeCustomer() {
        if (_queue.Count == 0) {
            Console.WriteLine("No Customers in Queue.");
            return;
        }

        var customer = _queue[0];
        _queue.RemoveAt(0);
        Console.WriteLine(customer);
    }

    /// <summary>
    /// Support the WriteLine function to provide a string representation of the
    /// customer service queue object. This is useful for debugging. If you have a 
    /// CustomerService object called cs, then you run Console.WriteLine(cs) to
    /// see the contents.
    /// </summary>
    /// <returns>A string representation of the queue</returns>
    public override string ToString() {
        return $"[size={_queue.Count} max_size={_maxSize} => " + string.Join(", ", _queue) + "]";
    }
}
