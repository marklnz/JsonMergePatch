using System;
using System.Text.Json;

namespace JsonMergePatch
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("JsonMergePatch console application");

            // TODO: Create an instance of an object, which we will use to test the patcher...
            var test = new TestObject()
            {
                Id = 1,
                Name = "A Test",
                HasBeenPatched = false,
                TimeOfPatch = DateTimeOffset.MinValue,
                TestArray = new TestChildObject[]
                {
                    new TestChildObject() { Index = 1, Name = "Test Child 1" },
                    new TestChildObject() { Index = 2, Name = "Test Child 2"}
                }
            };

            var prePatchJson = JsonSerializer.Serialize(test);

            Console.WriteLine($"Patch Object before patch application: {prePatchJson}");
            Console.WriteLine();

            // TODO: Then create a patch json for it
            var patch = new TestObject()
            {
                Id = 2,
                Name = "A Patched Test",
                HasBeenPatched = true,
                TimeOfPatch = DateTimeOffset.Now,
                TestArray = new TestChildObject[]
                {
                    new TestChildObject() { Index = 1, Name = "Test Child 1" },
                    new TestChildObject() { Index = 3, Name = "Test Child 3"}
                }
            };

            var patchJson = JsonSerializer.Serialize(patch);

            Console.WriteLine($"Patch Json: {patchJson}");
            Console.WriteLine();

            // TODO: Then run through the patcher.
            var postPatchJson = JsonMergePatch.Patch(prePatchJson, patchJson);

            TestObject patched = JsonSerializer.Deserialize<TestObject>(postPatchJson);

            // TODO: Test that the changes worked.
            //var postPatchJson = JsonSerializer.Serialize(test);
            Console.WriteLine($"Patch Object after patch application: {postPatchJson}");
            Console.WriteLine();

            if (postPatchJson == patchJson)
                Console.WriteLine("SUCCEEDED! Patched object json matches patch json");
            else
                Console.WriteLine("FAILED! Patched object json different from patch json");

            if (patched.Id == patch.Id && patched.Name == patched.Name && patched.HasBeenPatched == patch.HasBeenPatched &&
                patched.TimeOfPatch == patch.TimeOfPatch && patched.TestArray.Length == patch.TestArray.Length &&
                patched.TestArray.Length == 2 && patched.TestArray[0].Index == patch.TestArray[0].Index &&
                patched.TestArray[0].Name == patch.TestArray[0].Name && patched.TestArray[1].Index == patch.TestArray[1].Index &&
                patched.TestArray[1].Name == patch.TestArray[1].Name)
            {
                Console.WriteLine("SUCCEEDED! Deserialized object was patched correctly");
            }
            else
            {
                Console.WriteLine("FAILED! Deserialized object was not patched correctly");
            }
        }
    }

    public class TestObject
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool HasBeenPatched { get; set; }
        public DateTimeOffset TimeOfPatch { get; set; }
        public TestChildObject[] TestArray { get; set; }
    }

    public class TestChildObject
    {
        public int Index { get; set; }
        public string Name { get; set; }
    }
}
