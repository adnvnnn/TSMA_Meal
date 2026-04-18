window.submitLogoutForm = async function () {

    const res = await fetch("/logout", {
        method: "POST",
        credentials: "include"
    });

    if (res.ok) {
        window.location.href = "/";
    } else {
        window.location.href = "/";
    }
};