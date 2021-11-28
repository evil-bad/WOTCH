'use strict';

;const REQUEST = (function () {

    const CONFIG = {
        URL: "/Home/GetData",
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
        await sendCommmon(control, fetch);
    }

    async function sendConcurrent(control) {
        await sendCommmon(control, sendMultipleRequest);
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

    function sendMultipleRequest(url) {
        let concurrentRequestNumber = document.getElementById(CONFIG.concurrentRequestNumberID)?.value * 1 ?? CONFIG.defaultConcurrentNumber;

        let requestPromises = [];

        for (let i = 0; i < concurrentRequestNumber; i++) {
            requestPromises.push(fetch(url));
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

    return {
        sendNormal,
        sendConcurrent
    }
}());