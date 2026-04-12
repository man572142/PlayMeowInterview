using System.Collections.Generic;
using NUnit.Framework;
using PlayMeow.Editor;
using UnityEngine;

namespace PlayMeow.Tests
{
    public class ProjectNoteBookTests
    {
        private ProjectNoteBook _book;

        [SetUp]
        public void SetUp()
        {
            _book = ScriptableObject.CreateInstance<ProjectNoteBook>();
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_book);
        }

        [Test]
        public void GetNoteNames_EmptyList_ReturnsSingleNoNotesEntry()
        {
            _book.notes = new List<ProjectNoteData>();

            string[] names = _book.GetNoteNames();

            Assert.AreEqual(1, names.Length);
            Assert.AreEqual("(No Notes)", names[0]);
        }

        [Test]
        public void GetNoteNames_NullList_ReturnsSingleNoNotesEntry()
        {
            _book.notes = null;

            string[] names = _book.GetNoteNames();

            Assert.AreEqual(1, names.Length);
            Assert.AreEqual("(No Notes)", names[0]);
        }

        [Test]
        public void GetNoteNames_PopulatedList_ReturnsNames()
        {
            _book.notes = new List<ProjectNoteData>
            {
                new ProjectNoteData { name = "Alpha" },
                new ProjectNoteData { name = "Beta" }
            };

            string[] names = _book.GetNoteNames();

            Assert.AreEqual(2, names.Length);
            Assert.AreEqual("Alpha", names[0]);
            Assert.AreEqual("Beta", names[1]);
        }

        [Test]
        public void GetNoteNames_EmptyName_ReturnsUnnamedPlaceholder()
        {
            _book.notes = new List<ProjectNoteData>
            {
                new ProjectNoteData { name = "" }
            };

            string[] names = _book.GetNoteNames();

            Assert.AreEqual(1, names.Length);
            StringAssert.StartsWith("(Unnamed", names[0]);
        }
    }
}