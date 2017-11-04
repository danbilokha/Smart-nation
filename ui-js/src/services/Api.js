const API_PATH = '/';

function statusChecker(response) {
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
        .then(response => response.json());
        /* .then(json => {
            return {
                username: json.login
                , name: json.name
                , surname: json.surname
                , role: json.role
        }}); */
    }
};

export function registrationRequest(userData) {
    return function() {
        return fetch("http://localhost:50363/Account/Register", {
            method: "POST",
            headers: {
                Accept: 'application/json, text/javascript, */*; q=0.01',
                'Content-type': 'application/json; charset=UTF-8',
            },
            mode: "cors",
            credentials: "include",
            body: JSON.stringify(userData)
        })
        .then(statusChecker);
    }
}

export function logoutRequest() {
    
    //return function() {
        console.log("logout"); 
        return fetch("http://localhost:50363/Account/Logout", {
            method: "POST", 
            headers: {
                Accept: 'application/json, text/javascript, */*; q=0.01',
                'Content-type': 'application/json; charset=UTF-8',
            },
            mode: "cors",
            credentials: "include",
            body: JSON.stringify({
                Email: "das@mail.ru",
                Phone: "+380555",
                Password: "SDAS"
            })
        });
    //}
}
