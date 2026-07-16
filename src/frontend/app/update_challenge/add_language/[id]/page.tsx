import { redirect } from "next/navigation";
import { getSession } from "@/lib/session";
import { AddLanguageForm } from "./AddLanguageForm";
export default async function AddLanguagePage({params}:{params:Promise<{id:string}>}) {
    //todo: check if viewer is author
    const session = await getSession();
    if (!session) {
        redirect("/");
    }
    const{id}=await params;
    return <AddLanguageForm id={id}/>;
}