﻿using System;
using Wfm.Core.Domain.Accounts;
using Wfm.Core.Domain.Forums;
using Wfm.Tests;
using NUnit.Framework;

namespace Wfm.Data.Tests.Forums
{
    [TestFixture]
    public class ForumSubscriptionPersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_forum_subscription_forum_subscribed()
        {
            var account = GetTestAccount();
            var accountFromDb = SaveAndLoadEntity(account);
            accountFromDb.ShouldNotBeNull();

            var forumGroup = new ForumGroup
            {
                Name = "Forum Group 1",
                DisplayOrder = 1,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };

            var forumGroupFromDb = SaveAndLoadEntity(forumGroup);
            forumGroupFromDb.ShouldNotBeNull();
            forumGroupFromDb.Name.ShouldEqual("Forum Group 1");
            forumGroupFromDb.DisplayOrder.ShouldEqual(1);

            var forum = new Forum
            {
                ForumGroup = forumGroupFromDb,
                Name = "Forum 1",
                Description = "Forum 1 Description",
                ForumGroupId = forumGroupFromDb.Id,
                DisplayOrder = 10,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                NumPosts = 25,
                NumTopics = 15
            };

            forumGroup.Forums.Add(forum);
            var forumFromDb = SaveAndLoadEntity(forum);
            forumFromDb.ShouldNotBeNull();
            forumFromDb.Name.ShouldEqual("Forum 1");
            forumFromDb.Description.ShouldEqual("Forum 1 Description");
            forumFromDb.DisplayOrder.ShouldEqual(10);
            forumFromDb.NumTopics.ShouldEqual(15);
            forumFromDb.NumPosts.ShouldEqual(25);
            forumFromDb.ForumGroupId.ShouldEqual(forumGroupFromDb.Id);

            var forumTopic = new ForumTopic
            {
                Subject = "Forum Topic 1",
                Forum = forumFromDb,
                ForumId = forumFromDb.Id,
                TopicTypeId = (int)ForumTopicType.Sticky,
                Views = 123,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                NumPosts = 100,
                AccountId = accountFromDb.Id,
            };

            var forumTopicFromDb = SaveAndLoadEntity(forumTopic);
            forumTopicFromDb.ShouldNotBeNull();
            forumTopicFromDb.Subject.ShouldEqual("Forum Topic 1");
            forumTopicFromDb.Views.ShouldEqual(123);
            forumTopicFromDb.NumPosts.ShouldEqual(100);
            forumTopicFromDb.TopicTypeId.ShouldEqual((int)ForumTopicType.Sticky);
            forumTopicFromDb.ForumId.ShouldEqual(forumFromDb.Id);

            var forumSubscription = new ForumSubscription
            {
                CreatedOnUtc = DateTime.UtcNow,
                SubscriptionGuid = new Guid("11111111-2222-3333-4444-555555555555"),
                ForumId = forumFromDb.Id,
                AccountId = accountFromDb.Id,
            };

            var forumSubscriptionFromDb = SaveAndLoadEntity(forumSubscription);
            forumSubscriptionFromDb.ShouldNotBeNull();
            forumSubscriptionFromDb.SubscriptionGuid.ToString().ShouldEqual("11111111-2222-3333-4444-555555555555");
            forumSubscriptionFromDb.TopicId.ShouldEqual(0);
            forumSubscriptionFromDb.ForumId.ShouldEqual(forumFromDb.Id);
        }

        [Test]
        public void Can_save_and_load_forum_subscription_topic_subscribed()
        {
            var account = GetTestAccount();
            var accountFromDb = SaveAndLoadEntity(account);
            accountFromDb.ShouldNotBeNull();

            var forumGroup = new ForumGroup
            {
                Name = "Forum Group 1",
                DisplayOrder = 1,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };

            var forumGroupFromDb = SaveAndLoadEntity(forumGroup);
            forumGroupFromDb.ShouldNotBeNull();
            forumGroupFromDb.Name.ShouldEqual("Forum Group 1");
            forumGroupFromDb.DisplayOrder.ShouldEqual(1);

            var forum = new Forum
            {
                ForumGroup = forumGroupFromDb,
                Name = "Forum 1",
                Description = "Forum 1 Description",
                ForumGroupId = forumGroupFromDb.Id,
                DisplayOrder = 10,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                NumPosts = 25,
                NumTopics = 15
            };

            forumGroup.Forums.Add(forum);
            var forumFromDb = SaveAndLoadEntity(forum);
            forumFromDb.ShouldNotBeNull();
            forumFromDb.Name.ShouldEqual("Forum 1");
            forumFromDb.Description.ShouldEqual("Forum 1 Description");
            forumFromDb.DisplayOrder.ShouldEqual(10);
            forumFromDb.NumTopics.ShouldEqual(15);
            forumFromDb.NumPosts.ShouldEqual(25);
            forumFromDb.ForumGroupId.ShouldEqual(forumGroupFromDb.Id);

            var forumTopic = new ForumTopic
            {
                Subject = "Forum Topic 1",
                Forum = forumFromDb,
                ForumId = forumFromDb.Id,
                TopicTypeId = (int)ForumTopicType.Sticky,
                Views = 123,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                NumPosts = 100,
                AccountId = accountFromDb.Id,
            };

            var forumTopicFromDb = SaveAndLoadEntity(forumTopic);
            forumTopicFromDb.ShouldNotBeNull();
            forumTopicFromDb.Subject.ShouldEqual("Forum Topic 1");
            forumTopicFromDb.Views.ShouldEqual(123);
            forumTopicFromDb.NumPosts.ShouldEqual(100);
            forumTopicFromDb.TopicTypeId.ShouldEqual((int)ForumTopicType.Sticky);
            forumTopicFromDb.ForumId.ShouldEqual(forumFromDb.Id);

            var forumSubscription = new ForumSubscription
            {
                CreatedOnUtc = DateTime.UtcNow,
                SubscriptionGuid = new Guid("11111111-2222-3333-4444-555555555555"),
                TopicId = forumTopicFromDb.Id,
                AccountId = accountFromDb.Id,
            };

            var forumSubscriptionFromDb = SaveAndLoadEntity(forumSubscription);
            forumSubscriptionFromDb.ShouldNotBeNull();
            forumSubscriptionFromDb.SubscriptionGuid.ToString().ShouldEqual("11111111-2222-3333-4444-555555555555");
            forumSubscriptionFromDb.TopicId.ShouldEqual(forumTopicFromDb.Id);
            forumSubscriptionFromDb.ForumId.ShouldEqual(0);
        }

        protected Account GetTestAccount()
        {
            return new Account
            {
                AccountGuid = Guid.NewGuid(),
                //Note = "some comment here",
                IsActive = true,
                IsDeleted = false,
                CreatedOnUtc = new DateTime(2010, 01, 01),
                LastActivityDateUtc = new DateTime(2010, 01, 02)
            };
        }
    }
}
