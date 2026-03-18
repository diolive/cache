document.addEventListener("DOMContentLoaded", function() {
    const category = document.getElementById("CategoryId");
    const shopSuggestions = document.getElementById("shop-suggestions");
    const purchaseSuggestions = document.getElementById("purchase-name-suggestions");
    const nameInputs = Array.from(document.querySelectorAll(".typeahead-names"));

    if (!shopSuggestions || !purchaseSuggestions) {
        return;
    }

    fetchJson(CFG.shopListUrl)
        .then(function(items) {
            updateSuggestions(shopSuggestions, items);
        })
        .catch(function() { });

    const updatePurchaseSuggestions = debounce(function(query) {
        if (!query) {
            purchaseSuggestions.replaceChildren();
            return;
        }

        const url = new URL(CFG.purchaseListUrl, window.location.origin);
        url.searchParams.set("q", query);

        fetchJson(url)
            .then(function(items) {
                updateSuggestions(purchaseSuggestions, items);
            })
            .catch(function() { });
    }, 200);

    nameInputs.forEach(function(input) {
        input.addEventListener("input", function() {
            updatePurchaseSuggestions(input.value.trim());
        });

        input.addEventListener("change", function() {
            applyLatestCategory(input.value.trim());
        });

        input.addEventListener("blur", function() {
            applyLatestCategory(input.value.trim());
        });
    });

    function applyLatestCategory(purchaseName) {
        if (!purchaseName || !category) {
            return;
        }

        const url = new URL(CFG.latestCategoryUrl, window.location.origin);
        url.searchParams.set("purchase", purchaseName);

        fetchJson(url)
            .then(function(result) {
                if (result) {
                    category.value = result;
                }
            })
            .catch(function() { });
    }
});

function updateSuggestions(target, items) {
    target.replaceChildren();

    items.forEach(function(item) {
        const option = document.createElement("option");
        option.value = item;
        target.appendChild(option);
    });
}

function fetchJson(url) {
    return fetch(url).then(function(response) {
        if (!response.ok) {
            throw new Error("Request failed");
        }

        return response.json();
    });
}

function debounce(callback, delay) {
    let timeoutId = 0;

    return function() {
        const args = arguments;

        clearTimeout(timeoutId);
        timeoutId = window.setTimeout(function() {
            callback.apply(null, args);
        }, delay);
    };
}
