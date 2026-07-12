'use client';
import { useActionState } from "react";
import { handleLogin } from "./actions";
import { getSession } from "@/lib/session";
import { redirect } from "next/navigation";
export default function LoginForm(){
    const [state, formAction, isPending]=useActionState(handleLogin, null);
    return (
        <form action={formAction}>
            <input type="name" name="username" placeholder="Username"/>
            <input type="password" name="password" placeholder="Password"/>
            <button type="submit">Submit</button>
            {state?.message && (
                    <p style={{color:state.success? 'green': 'red'}}>
                        {state.message}
                    </p>
                )}
        </form>
        
    );
}