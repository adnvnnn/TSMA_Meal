window.loginWithOtp = async function (phone, code) {

    try {
        const formData = new FormData();
        formData.append("phoneNumber", phone);
        formData.append("code", code);

        const res = await fetch("/check-otp", {
            method: "POST",
            body: formData,
            credentials: "include"
        });

        if (!res.ok) {
            const errorText = await res.text();
            throw new Error(errorText || "Login failed");
        }

        window.location.href = "/";

    } catch (err) {

        return err.message;
    }
};