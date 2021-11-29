'use strict';

;const REQUEST = (function () {

    const CONFIG = {
        URL: "/Home/SendData",
        method: 'POST',
        concurrentRequestNumberID: 'concurrentRequestNumber',
        defaultConcurrentNumber: 5,
        class: {
            error: 'error'
        },
        attr: {
            disabled: 'disabled'
        }
    };

    async function sendNormal(control) {
        await sendCommmon(control, fetchWithData);
    }

    async function sendConcurrent(control) {
        await sendCommmon(control, sendMultiple);
    }

    async function sendCommmon(control, getResponseFunc) {
        onStart(control);

        try {
            let response = await getResponseFunc(CONFIG.URL);
            if (response.constructor !== Array)
                response = [response];

            let errors = response.filter((res) => res.status !== 200);

            if (errors.length)
                throw new Error();
            else
                onSuccess(control);
        } catch {
            onError(control);
        }
    }

    function sendMultiple(url) {
        let concurrentRequestNumber = document.getElementById(CONFIG.concurrentRequestNumberID)?.value * 1 ?? CONFIG.defaultConcurrentNumber;

        let requestPromises = [];

        for (let i = 0; i < concurrentRequestNumber; i++) {
            requestPromises.push(fetchWithData(url));
        }

        return Promise.all(requestPromises);
    }

    function onStart(control) {
        control.classList.remove(CONFIG.class.error);
        control.setAttribute(CONFIG.attr.disabled, CONFIG.attr.disabled);
    }

    function onSuccess(control) {
        control.removeAttribute(CONFIG.attr.disabled);
    }

    function onError(control) {
        control.classList.add(CONFIG.class.error);
        control.removeAttribute(CONFIG.attr.disabled);
    }

    function fetchWithData(url) {

        return fetch(url, {
            method: CONFIG.method,
            headers: {
                'Accept': 'application/json',
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ "Data": "Data" })
        })
    }

    return {
        sendNormal,
        sendConcurrent
    }
}());