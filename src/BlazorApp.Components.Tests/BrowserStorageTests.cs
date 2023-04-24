using Blazored.LocalStorage;
using Bunit;

namespace BlazorApp.Components.Tests
{
    public class BrowserStorageTests : TestContext
    {
        private readonly ILocalStorageService localStorageService;

        public BrowserStorageTests()
        {
            localStorageService = this.AddBlazoredLocalStorage();
        }

        [Fact]
        public async Task Successfully_Use_LocalStorage()
        {
            // given
            string key = "name";
            Guid expectedValue = Guid.NewGuid();

            // when
            await localStorageService.ClearAsync();
            await localStorageService.SetItemAsync(key, expectedValue);
            Guid result = await localStorageService.GetItemAsync<Guid>(key);

            // then
            Assert.True(result == expectedValue);
        }
    }
}
