'use client';

import { useState } from "react";
import { useActionState } from "react";
import { SendChallengeRequest } from "./actions";
export function ChallengeForm(){
    const [state,formAction,isPending]=useActionState(SendChallengeRequest,null);
    return(
        <div>
            
            <form action={formAction}>
                <input type="text" name="title" placeholder="Title"/>
                <input type="text" name="description" placeholder="Description"/>
                <p>You will be able to add another languages later.</p>
                <br></br>
                <button type="submit">Submit</button>
            </form>
            
            
            
        </div>
    );
}