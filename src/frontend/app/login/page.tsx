'use client';
import { useActionState } from "react";
import { handleLogin } from "./actions";

export default function Login(){
    const [state, formAction, isPending]=useActionState(handleLogin, null);
    return (
        <form action={formAction}>
            <input type="name" name="username" placeholder="Username"/>
            <input type="password" name="password" placeholder="Password"/>
            <button type="submit">Submit</button>
        </form>
    );
}