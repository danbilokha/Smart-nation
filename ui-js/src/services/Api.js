const API_PATH = '/';

function statusChecker(response) {
    console.log(response);
    if(response.status >= 200 && response.status < 300)
        return Promise.resolve(response);
    return Promise.reject(response);
}

export function loginRequest(userData) {
    return function () {
        return fetch("http://localhost:50363/Account/Login", {
            method: "POST",
            headers: {
                Accept: 'application/json, text/javascript, */*; q=0.01',
                'Content-type': 'application/json; charset=UTF-8',
            },
            mode: "cors",
            credentials: "include",
            body: JSON.stringify(userData)
        })
        .then(statusChecker)
        .then(response => response.json())
        //.then(json => console.log(json));
        /* return {
            username: "beokha",
            name: "Danil",
            surname: "Bilokha",
            role: "admin"
        } */
    }
};
