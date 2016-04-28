using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DbUp.Helpers;
using NUnit.Framework;

namespace DbUp.Tests.Helpers
{
    [TestFixture]
    public class SqlCommentRangeFinderTests
    {
        [Test]
        public void finds_nothing_when_no_comments()
        {
            Assert.AreEqual(0, SqlCommentRangeFinder.FindRanges("asdasdasd").Count);
        }
        [Test]
        public void finds_one_empty()
        {
            Assert.AreEqual(1, SqlCommentRangeFinder.FindRanges("/**/asdasdasd").Count);
        }

        [Test]
        public void finds_one_non_empty()
        {
            Assert.AreEqual(1, SqlCommentRangeFinder.FindRanges("/*asd*/asdasdasd").Count);
        }

        [Test]
        public void doesnt_misidentify_non_comment()
        {
            Assert.AreEqual(0, SqlCommentRangeFinder.FindRanges("asd*/asdasdasd").Count);
        }

        [Test]
        public void finds_one_nested_empty()
        {
            var comments = SqlCommentRangeFinder.FindRanges("/*/**/asd*/asdasdasd");
            Assert.AreEqual(1, comments.Count);
            Assert.AreEqual(0, comments[0].Key);
            Assert.AreEqual(10, comments[0].Value);
        }

        [Test]
        public void finds_one_nested_non_empty()
        {
            var comments = SqlCommentRangeFinder.FindRanges("/*/*asd*/asd*/asdasdasd");
            Assert.AreEqual(1, comments.Count);
            Assert.AreEqual(0, comments[0].Key);
            Assert.AreEqual(13, comments[0].Value);
        }

        [Test]
        public void finds_one_with_line_breaks()
        {
            var comments = SqlCommentRangeFinder.FindRanges("/* \r\n */asdasdasd");
            Assert.AreEqual(1, comments.Count);
            Assert.AreEqual(0, comments[0].Key);
            Assert.AreEqual(7, comments[0].Value);
        }

        [Test]
        public void finds_three()
        {
            var comments = SqlCommentRangeFinder.FindRanges("/**/asdasdasd/**/asd/**/");
            Assert.AreEqual(3, comments.Count);
            Assert.AreEqual(0, comments[0].Key);
            Assert.AreEqual(3, comments[0].Value);
            Assert.AreEqual(13, comments[1].Key);
            Assert.AreEqual(16, comments[1].Value);
            Assert.AreEqual(20, comments[2].Key);
            Assert.AreEqual(23, comments[2].Value);
        }

        [Test]
        public void finds_line_comments()
        {
            var comments = SqlCommentRangeFinder.FindRanges("-- \r\n --asdasdasd");
            Assert.AreEqual(2, comments.Count);
            Assert.AreEqual(0, comments[0].Key);
            Assert.AreEqual(4, comments[0].Value);
            Assert.AreEqual(6, comments[1].Key);
            Assert.AreEqual(16, comments[1].Value);
        }

        [Test]
        public void ignores_line_comment_inside_block_comment()
        {
            Assert.AreEqual(1, SqlCommentRangeFinder.FindRanges("/*--*/asdasdasd").Count);
        }

        [Test]
        public void ignores_block_comment_inside_line_comment()
        {
            Assert.AreEqual(1, SqlCommentRangeFinder.FindRanges("--/**/asdasdasd").Count);
            Assert.AreEqual(1, SqlCommentRangeFinder.FindRanges("--*/asdasdasd").Count);
            Assert.AreEqual(1, SqlCommentRangeFinder.FindRanges("--/*asdasdasd").Count);
        }


        [Test]
        public void matches_unclosed_block_comment()
        {
            Assert.AreEqual(1, SqlCommentRangeFinder.FindRanges("/*--asdasdasd").Count);
        }
    }
}
