function postData(url, data) {
    const body = {
        method: "POST",
        mode: "cors",
        cache: "no-cache",
        credentials: "include",
        headers: {
            "Content-Type": "application/json"
        },
        redirect: "follow"
    };

    if (data) {
        body.body = JSON.stringify(data);
    };

    return fetch(url, body);
}