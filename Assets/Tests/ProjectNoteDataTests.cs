using NUnit.Framework;
using PlayMeow.Editor;

namespace PlayMeow.Tests
{
    public class ProjectNoteDataTests
    {
        [Test]
        public void DefaultName_IsNewNote()
        {
            var note = new ProjectNoteData();

            Assert.AreEqual("New Note", note.name);
        }

        [Test]
        public void Description_DefaultsToNull()
        {
            var note = new ProjectNoteData();

            Assert.IsNull(note.description);
        }
    }
}