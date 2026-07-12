'use client';
import { useActionState } from "react";
import { handleRegister } from "./actions";
import { redirect } from "next/navigation";
import { getSession } from "@/lib/session";

export default function RegisterForm(){
    
    const[state,formAction,isPending]=useActionState(handleRegister,null); //we use useactionstate because we want a return from backend(so we cant just do action=handleRegister)
    return(
            <form action={formAction}>
                
                <input type="text" name="username" placeholder="Username"/>
                <input type="email" name="email" placeholder="Email"/>
                <input type="password" name="password" placeholder="Password"/>
                <input type="password" name="password-verify" placeholder="Verify Password"/>
                <button type="submit">Register</button>
                {state?.message && (
                    <p style={{color:state.success? 'green': 'red'}}>
                        {state.message}
                    </p>
                )}

            </form>
        
    );
};